using UnityEngine;

public class SpanTimer
{
    private float span;

    private float nextTime;

    private bool pause;

    public SpanTimer(float span)
    {
        this.span = span;
    }

    public bool IsReady()
    {
        if (this.span < 0f || this.pause)
        {
            return false;
        }
        if (this.span == 0f)
        {
            return true;
        }
        if (this.nextTime > Time.time)
        {
            return false;
        }
        this.ResetNextTime();
        return true;
    }

    public void ResetNextTime()
    {
        this.nextTime = Time.time + this.span;
    }

    public void SetTempSpan(float temp_span)
    {
        this.nextTime = Time.time + temp_span;
    }

    public void PauseOn()
    {
        this.pause = true;
    }

    public void PauseOff()
    {
        this.pause = false;
    }
}