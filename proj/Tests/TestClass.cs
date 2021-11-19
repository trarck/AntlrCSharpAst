class AA
{
	public int a;
	public float b;

	public void Test()
	{
		a = 1;
	}

	public void TestB()
	{
		b = 0.1f;
	}
}

class BB
{
	public void Test()
	{
		AA aa = new AA();
		aa.a = 2;
	}
}