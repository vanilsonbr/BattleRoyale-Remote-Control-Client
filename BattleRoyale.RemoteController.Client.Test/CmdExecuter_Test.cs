using BatteRoyale.RemoteController.Client;
using System;
using Xunit;

namespace BattleRoyale.RemoteController.Client.Test
{
    /// <summary>
    /// Run With administrator privileges
    /// </summary>
    public class CmdExecuter_Test
    {
        [Theory]
        [InlineData(@"cd .")]
        [InlineData(@"cd ..")] // cd C:\.. = C:\
        [InlineData(@"cd users/..")]
        [InlineData(@"cd /users/..")]
        [InlineData(@"cd /users/../")]
        public void ShoudReturnTheSameDirectory(string value)
        {
            string workingDirectory = @"C:\";

            CmdExecuter cmdExecuter = new CmdExecuter(workingDirectory);

            var result = cmdExecuter.ExecuteCommand(value);

            Assert.NotNull(result);

            Assert.True(result.Success,"result success should be true");

            Assert.Equal(workingDirectory, result.WorkingDirectory);

        }

        [Fact]
        public void ShoudReturnToDirectoryCFromCUsers()
        {
            string workingDirectory = @"C:\users";
            string expectedDirectory = @"C:\";

            CmdExecuter cmdExecuter = new CmdExecuter(workingDirectory);

            var result = cmdExecuter.ExecuteCommand("cd ..");

            Assert.NotNull(result);
            Assert.Equal(expectedDirectory, result.WorkingDirectory);
            Assert.True(result.Success);
        }

        [Fact]
        public void ShouldReturnToSameDirectoryFromC()
        {
            string workingDirectory = @"C:\";
            string expectedDirectory = @"C:\";

            CmdExecuter cmdExecuter = new CmdExecuter(workingDirectory);

            var result = cmdExecuter.ExecuteCommand("cd users/..");

            Assert.NotNull(result);
            Assert.Equal(expectedDirectory, result.WorkingDirectory);
            Assert.True(result.Success);
        }

        [Fact]
        public void SholdExecuteCommandDir()
        {
            string woorkingDirectory = @"C:\";

            CmdExecuter cmdExecuter = new CmdExecuter(woorkingDirectory);
            var result = cmdExecuter.ExecuteCommand("dir");

            Assert.NotNull(result);
            Assert.Equal(woorkingDirectory, result.WorkingDirectory);
            Assert.NotEmpty(result.Result);
            Assert.True(result.Success);
        }

        [Fact]
        public void ShouldReturnErrorAtInvalidDirectory()
        {
            string woorkingDirectory = @"C:\";

            CmdExecuter cmdExecuter = new CmdExecuter(woorkingDirectory);
            var result = cmdExecuter.ExecuteCommand("cd folderThatDoNotExistInTheSystem");

            Assert.NotNull(result);
            Assert.Equal(woorkingDirectory, result.WorkingDirectory);
            Assert.NotEmpty(result.Result);

            Assert.Equal("O sistema não pode encontrar o caminho especificado.", result.Result);
            Assert.False(result.Success);
        }

    }
}
