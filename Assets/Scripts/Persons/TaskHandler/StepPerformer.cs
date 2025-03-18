public class StepPerformer
{
    private Step step;
    private bool isFinished;

    public Step Step => step;
    public StepPerformer(Step step)
    {
        this.step = step;
        isFinished = false;
    }

    public bool IsFinished => isFinished;
    public void SetIsFinished(bool isFinished)
    {
        this.isFinished = isFinished;
    }

}