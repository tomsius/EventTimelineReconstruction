namespace EventTimelineReconstruction.UnitTests;

public class STATestMethodAttribute : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        {
            return this.Invoke(testMethod);
        }

        TestResult[] result = null;
        var thread = new Thread(() => result = this.Invoke(testMethod));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        return result;
    }

    private TestResult[] Invoke(ITestMethod testMethod)
    {
        return new[] { testMethod.Invoke(null) };
    }
}