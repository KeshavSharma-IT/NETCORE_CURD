namespace CURDTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            //Arange
            MyMath myMath = new MyMath();
             int input1=10, input2=5;
            int expeced = 15;

            // Act
            int actualval=myMath.Add(input1, input2);

            //Assert
            Assert.Equal(expeced, actualval);

        }
    }
}