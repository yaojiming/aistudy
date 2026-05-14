using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using AiTutor.Maui.Models;
using AiTutor.Maui.Services;
using Microsoft.Maui.Graphics;

namespace AiTutor.Maui.Controls;

/// <summary>
/// 在 AspectFit 图片上绘制题目区域框，并把点击转换为题目区域选择。
/// </summary>
public sealed class QuestionRegionOverlayView : GraphicsView, IDrawable
{
    public static readonly BindableProperty RegionsProperty = BindableProperty.Create(
        nameof(Regions),
        typeof(IEnumerable),
        typeof(QuestionRegionOverlayView),
        null,
        propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty SelectedRegionIdProperty = BindableProperty.Create(
        nameof(SelectedRegionId),
        typeof(string),
        typeof(QuestionRegionOverlayView),
        null,
        propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty ImagePixelWidthProperty = BindableProperty.Create(
        nameof(ImagePixelWidth),
        typeof(float),
        typeof(QuestionRegionOverlayView),
        0f,
        propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty ImagePixelHeightProperty = BindableProperty.Create(
        nameof(ImagePixelHeight),
        typeof(float),
        typeof(QuestionRegionOverlayView),
        0f,
        propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty RegionTappedCommandProperty = BindableProperty.Create(
        nameof(RegionTappedCommand),
        typeof(ICommand),
        typeof(QuestionRegionOverlayView));

    public QuestionRegionOverlayView()
    {
        Drawable = this;
        BackgroundColor = Colors.Transparent;

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GestureRecognizers.Add(tap);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        AttachCollectionChanged();
    }

    public IEnumerable? Regions
    {
        get => (IEnumerable?)GetValue(RegionsProperty);
        set => SetValue(RegionsProperty, value);
    }

    public string? SelectedRegionId
    {
        get => (string?)GetValue(SelectedRegionIdProperty);
        set => SetValue(SelectedRegionIdProperty, value);
    }

    public float ImagePixelWidth
    {
        get => (float)GetValue(ImagePixelWidthProperty);
        set => SetValue(ImagePixelWidthProperty, value);
    }

    public float ImagePixelHeight
    {
        get => (float)GetValue(ImagePixelHeightProperty);
        set => SetValue(ImagePixelHeightProperty, value);
    }

    public ICommand? RegionTappedCommand
    {
        get => (ICommand?)GetValue(RegionTappedCommandProperty);
        set => SetValue(RegionTappedCommandProperty, value);
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (ImagePixelWidth <= 0 || ImagePixelHeight <= 0)
        {
            return;
        }

        foreach (var region in GetRegions())
        {
            var rect = ImageCoordinateMapper.ImageRectToViewRect(region.Bounds, ImagePixelWidth, ImagePixelHeight, dirtyRect.Width, dirtyRect.Height);
            var isSelected = string.Equals(region.Id, SelectedRegionId, StringComparison.Ordinal);

            var strokeColor = GetStrokeColor(region, isSelected);
            canvas.StrokeColor = strokeColor;
            canvas.StrokeSize = isSelected ? 5 : region.Status == HomeworkQuestionStatus.Checking ? 4 : 2;
            canvas.FillColor = isSelected
                ? Color.FromRgba(56, 189, 248, 44)
                : GetFillColor(region);
            canvas.FillRoundedRectangle(rect, 10);
            canvas.DrawRoundedRectangle(rect, 10);

            canvas.FontColor = Colors.White;
            canvas.FontSize = 13;
            canvas.FillColor = isSelected ? Color.FromArgb("#0284C7") : GetLabelColor(region);
            var labelRect = new RectF(rect.Left, MathF.Max(0, rect.Top - 24), 72, 22);
            canvas.FillRoundedRectangle(labelRect, 8);
            canvas.DrawString(region.Title, labelRect, HorizontalAlignment.Center, VerticalAlignment.Center);

            DrawStatusBadge(canvas, region, rect);
        }
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        var point = e.GetPosition(this);
        if (point is null || Width <= 0 || Height <= 0)
        {
            return;
        }

        var viewPoint = new PointF((float)point.Value.X, (float)point.Value.Y);
        var selected = GetRegions()
            .LastOrDefault(region => ImageCoordinateMapper.IsViewPointInsideImageRect(
                viewPoint,
                region.Bounds,
                ImagePixelWidth,
                ImagePixelHeight,
                (float)Width,
                (float)Height));

        if (selected is not null && RegionTappedCommand?.CanExecute(selected.Id) == true)
        {
            RegionTappedCommand.Execute(selected.Id);
        }
    }

    private IReadOnlyList<QuestionRegion> GetRegions()
    {
        return Regions?.OfType<QuestionRegion>().ToList() ?? [];
    }

    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (QuestionRegionOverlayView)bindable;
        view.AttachCollectionChanged();
        view.Invalidate();
    }

    private INotifyCollectionChanged? _attachedCollection;

    private void AttachCollectionChanged()
    {
        if (ReferenceEquals(_attachedCollection, Regions))
        {
            return;
        }

        if (_attachedCollection is not null)
        {
            _attachedCollection.CollectionChanged -= OnRegionsChanged;
        }

        _attachedCollection = Regions as INotifyCollectionChanged;
        if (_attachedCollection is not null)
        {
            _attachedCollection.CollectionChanged += OnRegionsChanged;
        }
    }

    private void OnRegionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Invalidate();
    }

    private static Color GetStrokeColor(QuestionRegion region, bool isSelected)
    {
        if (isSelected)
        {
            return Color.FromArgb("#38BDF8");
        }

        return region.Status switch
        {
            HomeworkQuestionStatus.Checking => Color.FromArgb("#F59E0B"),
            HomeworkQuestionStatus.Correct => Color.FromArgb("#22C55E"),
            HomeworkQuestionStatus.Wrong => Color.FromArgb("#EF4444"),
            HomeworkQuestionStatus.Uncertain => Color.FromArgb("#A3A3A3"),
            _ => Color.FromArgb("#60A5FA")
        };
    }

    private static Color GetFillColor(QuestionRegion region)
    {
        return region.Status switch
        {
            HomeworkQuestionStatus.Checking => Color.FromRgba(245, 158, 11, 40),
            HomeworkQuestionStatus.Correct => Color.FromRgba(34, 197, 94, 34),
            HomeworkQuestionStatus.Wrong => Color.FromRgba(239, 68, 68, 34),
            HomeworkQuestionStatus.Uncertain => Color.FromRgba(163, 163, 163, 34),
            _ => Color.FromRgba(96, 165, 250, 30)
        };
    }

    private static Color GetLabelColor(QuestionRegion region)
    {
        return region.Status switch
        {
            HomeworkQuestionStatus.Checking => Color.FromArgb("#B45309"),
            HomeworkQuestionStatus.Correct => Color.FromArgb("#15803D"),
            HomeworkQuestionStatus.Wrong => Color.FromArgb("#B91C1C"),
            HomeworkQuestionStatus.Uncertain => Color.FromArgb("#525252"),
            _ => Color.FromArgb("#2563EB")
        };
    }

    private static void DrawStatusBadge(ICanvas canvas, QuestionRegion region, RectF rect)
    {
        var badgeText = region.Status switch
        {
            HomeworkQuestionStatus.Checking => "...",
            HomeworkQuestionStatus.Correct => "✓",
            HomeworkQuestionStatus.Wrong => "×",
            HomeworkQuestionStatus.Uncertain => "?",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(badgeText))
        {
            return;
        }

        var badgeSize = 28f;
        var badgeRect = new RectF(rect.Right - badgeSize - 4, rect.Top + 4, badgeSize, badgeSize);
        canvas.FillColor = GetLabelColor(region);
        canvas.FillEllipse(badgeRect);
        canvas.FontColor = Colors.White;
        canvas.FontSize = 18;
        canvas.DrawString(badgeText, badgeRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
