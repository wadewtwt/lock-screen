using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Size = System.Windows.Size;

namespace LockScreen.App.Controls;

public class DotMatrixTextBlock : FrameworkElement
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty DotSizeProperty =
        DependencyProperty.Register(nameof(DotSize), typeof(double), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(8d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty DotGapProperty =
        DependencyProperty.Register(nameof(DotGap), typeof(double), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(4d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CharacterSpacingProperty =
        DependencyProperty.Register(nameof(CharacterSpacing), typeof(int), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty LineSpacingProperty =
        DependencyProperty.Register(nameof(LineSpacing), typeof(int), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OnBrushProperty =
        DependencyProperty.Register(nameof(OnBrush), typeof(Brush), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(System.Windows.Media.Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OffBrushProperty =
        DependencyProperty.Register(nameof(OffBrush), typeof(Brush), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromArgb(24, 255, 255, 255)), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty GlowBrushProperty =
        DependencyProperty.Register(nameof(GlowBrush), typeof(Brush), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 255, 255, 255)), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowOffDotsProperty =
        DependencyProperty.Register(nameof(ShowOffDots), typeof(bool), typeof(DotMatrixTextBlock),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double DotSize
    {
        get => (double)GetValue(DotSizeProperty);
        set => SetValue(DotSizeProperty, value);
    }

    public double DotGap
    {
        get => (double)GetValue(DotGapProperty);
        set => SetValue(DotGapProperty, value);
    }

    public int CharacterSpacing
    {
        get => (int)GetValue(CharacterSpacingProperty);
        set => SetValue(CharacterSpacingProperty, value);
    }

    public int LineSpacing
    {
        get => (int)GetValue(LineSpacingProperty);
        set => SetValue(LineSpacingProperty, value);
    }

    public Brush OnBrush
    {
        get => (Brush)GetValue(OnBrushProperty);
        set => SetValue(OnBrushProperty, value);
    }

    public Brush OffBrush
    {
        get => (Brush)GetValue(OffBrushProperty);
        set => SetValue(OffBrushProperty, value);
    }

    public Brush GlowBrush
    {
        get => (Brush)GetValue(GlowBrushProperty);
        set => SetValue(GlowBrushProperty, value);
    }

    public bool ShowOffDots
    {
        get => (bool)GetValue(ShowOffDotsProperty);
        set => SetValue(ShowOffDotsProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return CalculateDesiredSize();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return finalSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var lines = NormalizeLines();
        if (lines.Length == 0)
        {
            return;
        }

        var metrics = GetMetrics();
        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            for (var charIndex = 0; charIndex < line.Length; charIndex++)
            {
                var character = line[charIndex];
                var originX = charIndex * metrics.CharAdvance;
                var originY = lineIndex * metrics.LineAdvance;

                for (var y = 0; y < DotMatrixFont.GlyphHeight; y++)
                {
                    for (var x = 0; x < DotMatrixFont.GlyphWidth; x++)
                    {
                        var rect = new Rect(
                            originX + x * metrics.CellAdvance,
                            originY + y * metrics.CellAdvance,
                            metrics.DotSize,
                            metrics.DotSize);

                        var isOn = DotMatrixFont.IsPixelOn(character, x, y);
                        if (!isOn && !ShowOffDots)
                        {
                            continue;
                        }

                        if (isOn)
                        {
                            var glowRect = new Rect(
                                rect.X - metrics.GlowPadding,
                                rect.Y - metrics.GlowPadding,
                                rect.Width + metrics.GlowPadding * 2,
                                rect.Height + metrics.GlowPadding * 2);
                            drawingContext.DrawEllipse(GlowBrush, null, glowRect.Location + new Vector(glowRect.Width / 2, glowRect.Height / 2), glowRect.Width / 2, glowRect.Height / 2);
                        }

                        var brush = isOn ? OnBrush : OffBrush;
                        drawingContext.DrawEllipse(brush, null, rect.Location + new Vector(rect.Width / 2, rect.Height / 2), rect.Width / 2, rect.Height / 2);
                    }
                }
            }
        }
    }

    private Size CalculateDesiredSize()
    {
        var lines = NormalizeLines();
        if (lines.Length == 0)
        {
            return new Size(0, 0);
        }

        var metrics = GetMetrics();
        var maxColumns = lines.Max(static line => line.Length);

        var width = maxColumns == 0
            ? 0
            : maxColumns * metrics.CharAdvance - metrics.CharacterSpacingWidth;

        var height = lines.Length * metrics.LineAdvance - metrics.LineSpacingHeight;

        return new Size(width, height);
    }

    private string[] NormalizeLines()
    {
        var source = Text ?? string.Empty;
        return source.Replace("\r", string.Empty).Split('\n');
    }

    private DotMetrics GetMetrics()
    {
        var dotSize = Math.Max(1, DotSize);
        var dotGap = Math.Max(0, DotGap);
        var cellAdvance = dotSize + dotGap;
        var characterSpacingWidth = Math.Max(0, CharacterSpacing) * cellAdvance;
        var lineSpacingHeight = Math.Max(0, LineSpacing) * cellAdvance;
        var charAdvance = DotMatrixFont.GlyphWidth * cellAdvance + characterSpacingWidth;
        var lineAdvance = DotMatrixFont.GlyphHeight * cellAdvance + lineSpacingHeight;

        return new DotMetrics(dotSize, cellAdvance, charAdvance, lineAdvance, characterSpacingWidth, lineSpacingHeight, dotSize * 0.4);
    }

    private readonly record struct DotMetrics(
        double DotSize,
        double CellAdvance,
        double CharAdvance,
        double LineAdvance,
        double CharacterSpacingWidth,
        double LineSpacingHeight,
        double GlowPadding);
}
