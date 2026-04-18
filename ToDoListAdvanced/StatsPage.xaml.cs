namespace ToDoListAdvanced;

public partial class StatsPage : ContentPage
{
	private Stats drawable = new Stats();
	public StatsPage()
	{
		InitializeComponent();
        statsView.Drawable = drawable;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        statsView.Invalidate();
        if (App.GlobalTasks != null)
        {
            var incomplete = App.GlobalTasks.Where(t => !t.Complete).ToList();
            InComplete.ItemsSource = incomplete;
        }
        else
        {
            InComplete.ItemsSource = new List<ToDoTask>();
        }
    }
}
public class Stats : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float angle = App.GlobalTasks?.Count > 0 ? ((float)App.GlobalTasks?.Count(t => t.Complete) / (float)App.GlobalTasks?.Count * 360) : 0;

        float size = Math.Min(dirtyRect.Width, dirtyRect.Height) - 40 * 2;
        float x = dirtyRect.Center.X - size / 2;
        float y = dirtyRect.Center.Y - size / 2;

        canvas.StrokeColor = Color.FromArgb("D600AA");
        canvas.StrokeSize = 14;
        canvas.DrawEllipse(x, y, size, size);

        if (App.GlobalTasks?.Count > 0)
        {
            canvas.StrokeColor = Color.FromArgb("C4E4E9");
            canvas.StrokeSize = 18;
            canvas.DrawArc(x, y, size, size, 90, 90 - angle, true, false);
            if (angle >= 360)
            {
                canvas.StrokeColor = Color.FromArgb("C4E4E9");
                canvas.StrokeSize = 18;
                canvas.DrawEllipse(x, y, size, size);
            }
        }

        canvas.FontSize = 32;
        double percent = App.GlobalTasks?.Count > 0 ? Math.Round((double)App.GlobalTasks?.Count(t => t.Complete) / (double)App.GlobalTasks?.Count * 100, 1) : 0;
        canvas.DrawString(percent + "%", dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}