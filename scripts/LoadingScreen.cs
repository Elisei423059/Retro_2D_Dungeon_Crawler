using Godot;

/// <summary>
/// Asynchronously loads heavy scenes in a background thread while updating a UI progress bar.
/// </summary>
public partial class LoadingScreen : Control
{
    [Export] private ProgressBar progressBar;
    [Export] private Label progressLabel;

    private string targetScenePath;
    private Godot.Collections.Array progressArray = new Godot.Collections.Array();
    private bool isLoading = false;

    /// <summary>
    /// Starts a threaded background load request for the specified scene path.
    /// </summary>
    public void LoadScene(string scenePath)
    {
        targetScenePath = scenePath;
        isLoading = true;

        // Initiate background resource loading
        Error err = ResourceLoader.LoadThreadedRequest(targetScenePath);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Error initiating threaded load for: {targetScenePath}");
            isLoading = false;
        }
    }

    public override void _Process(double delta)
    {
        if (!isLoading) return;

        // Poll the background thread for current loading status and progress percentage
        var status = ResourceLoader.LoadThreadedGetStatus(targetScenePath, progressArray);

        switch (status)
        {
            case ResourceLoader.ThreadLoadStatus.InProgress:
                // Extract normalized progress (0.0 to 1.0) and update UI
                if (progressArray.Count > 0)
                {
                    float progressValue = (float)progressArray[0];
                    UpdateUI(progressValue);
                }
                break;

            case ResourceLoader.ThreadLoadStatus.Loaded:
                // Loading complete: finalize UI at 100% and switch to the loaded PackedScene
                isLoading = false;
                UpdateUI(1.0f);

                var packedScene = (PackedScene)ResourceLoader.LoadThreadedGet(targetScenePath);
                GetTree().ChangeSceneToPacked(packedScene);
                break;

            case ResourceLoader.ThreadLoadStatus.Failed:
            case ResourceLoader.ThreadLoadStatus.InvalidResource:
                GD.PrintErr($"Threaded loading failed for: {targetScenePath}");
                isLoading = false;
                break;
        }
    }

    /// <summary>
    /// Updates the ProgressBar value and percentage Label text.
    /// </summary>
    private void UpdateUI(float progress)
    {
        int percentage = Mathf.RoundToInt(progress * 100);

        if (progressBar != null)
        {
            progressBar.Value = percentage;
        }

        if (progressLabel != null)
        {
            progressLabel.Text = $"Loading: {percentage}%";
        }
    }
}