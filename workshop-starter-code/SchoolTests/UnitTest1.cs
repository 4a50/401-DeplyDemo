using SchoolDemo.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SchoolTests
{
  public class UnitTest1 : Mock
  {
    [Fact]
    public async Task CanEnrollAndDropAStudent()
    {
      //Arrange
      var student = await CreateAndSaveTestStudent();
      var course = await CreateAndSaveTestCourse();

      var repository = new CourseRepository(_db);

      //Act
      await repository.AddStudent(course.Id, student.Id);

      //Assertions
      var actualCourse = await repository.GetOne(course.Id);
      Assert.Contains(actualCourse.Enrollments, e => e.StudentId == student.Id);

      //Act
      await repository.RemoveStudentFromCourse(course.Id, student.Id);
      Assert.DoesNotContain(actualCourse.Enrollments, e => e.StudentId == student.Id);

    }
    
  }
}
