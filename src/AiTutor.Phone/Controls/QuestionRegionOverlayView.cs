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

    private INotifyCollectionChanged? _attachedCollection;

    public QuestionRegionOverlayView()
    {
        Drawable = this;
        BackgroundColor = Colors.Transparent;

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GestureRecognizers.Add(tap);
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

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        AttachCollectionChanged();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (ImagePixelWidth <= 0 || ImagePixelHeight <= 0)
        {
            return;
        }

        foreach (var region in GetRegions())
        {
            var rect = ImageCoordinateMapper.ImageRectToViewRect(
                region.Bounds,
                ImagePixelWidth,
                ImagePixelHeight,
                dirtyRect.Width,
                dirtyRect.Height);

            if (rect.Width <= 1 || rect.Height <= 1)
            {
                continue;
            }

            var isSelected = string.Equals(region.Id, SelectedRegionId, StringComparison.Ordinal);
            canvas.StrokeColor = GetStrokeColor(region, isSelected);
            canvas.StrokeSize = isSelected ? 5 : region.Status == HomeworkQuestionStatus.Checking ? 4 : 2;
            canvas.FillColor = isSelected ? Color.FromRgba(56, 189, 248, 44) : GetFillColor(region);
            canvas.FillRoundedRectangle(rect, 10);
            canvas.DrawRoundedRectangle(rect, 10);

            DrawQuestionLabel(canvas, region, rect, dirtyRect);
            DrawStatusBadge(canvas, region, rect, dirtyRect);
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

    private static void DrawQuestionLabel(ICanvas canvas, QuestionRegion region, RectF rect, RectF bounds)
    {
        var labelWidth = MathF.Min(74, MathF.Max(48, rect.Width));
        var labelRect = new RectF(
            Math.Clamp(rect.Left, 0, MathF.Max(0, bounds.Width - labelWidth)),
            Math.Clamp(rect.Top - 24, 0, MathF.Max(0, bounds.Height - 22)),
            labelWidth,
            22);

        canvas.FillColor = GetLabelColor(region);
        canvas.FillRoundedRectangle(labelRect, 8);
        canvas.FontColor = Colors.White;
        canvas.FontSize = 13;
        canvas.DrawString(region.Title, labelRect, HorizontalAlignment.Center, VerticalAlignment.Center);
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

    private static void DrawStatusBadge(ICanvas canvas, QuestionRegion region, RectF rect, RectF bounds)
    {
        var badgeText = region.Status switch
        {
            HomeworkQuestionStatus.Checking => "...",
            HomeworkQuestionStatus.Correct => "\u2713",
            HomeworkQuestionStatus.Wrong => "\u00D7",
            HomeworkQuestionStatus.Uncertain => "?",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(badgeText))
        {
            return;
        }

        var badgeSize = 28f;
        var badgeLeft = Math.Clamp(rect.Right - badgeSize - 4, 0, MathF.Max(0, bounds.Width - badgeSize));
        var badgeTop = Math.Clamp(rect.Top + 4, 0, MathF.Max(0, bounds.Height - badgeSize));
        var badgeRect = new RectF(badgeLeft, badgeTop, badgeSize, badgeSize);

        canvas.FillColor = GetLabelColor(region);
        canvas.FillEllipse(badgeRect);
        canvas.FontColor = Colors.White;
        canvas.FontSize = region.Status == HomeworkQuestionStatus.Checking ? 12 : 18;
        canvas.DrawString(badgeText, badgeRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
