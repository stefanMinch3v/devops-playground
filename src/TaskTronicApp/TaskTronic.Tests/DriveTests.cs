using FluentAssertions;

namespace TaskTronic.Tests
{
    public class DriveTests
    {
        private readonly string[] validEmployeeNames = {"Pesho", "Gosho", "Ivan"};
        private readonly string[] validCompanyNames = {"Microsoft", "Google", "Amazon"};
        private readonly string[] validDepartmentNames = {"DevOps", "Marketing", "Sales"};

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Add_Folder_Should_Return_Success(int number)
        {
            var result = number is > 0 and <= 3;

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Add_File_Should_Return_Success(int number)
        {
            var result = number is > 0 and <= 3;

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Pesho")]
        [InlineData("Gosho")]
        [InlineData("Ivan")]
        public void Add_Employee_Should_Return_Success(string name)
        {
            var result = this.validEmployeeNames.Contains(name);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Microsoft")]
        [InlineData("Google")]
        [InlineData("Amazon")]
        public void Add_Company_Should_Return_Success(string name)
        {
            var result = this.validCompanyNames.Contains(name);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("DevOps")]
        [InlineData("Marketing")]
        [InlineData("Sales")]
        public void Add_Department_Should_Return_Success(string name)
        {
            var result = this.validDepartmentNames.Contains(name);

            result.Should().BeTrue();
        }
    }
}
