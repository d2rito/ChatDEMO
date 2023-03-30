namespace TestsProject
{
    [TestClass]
    public class AccountControllerTest
    {
        private AccountController _accountController;
        private Mock<UserManager<IdentityUser>> _userManagerMock;

        [TestInitialize]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            _accountController = new AccountController(_userManagerMock.Object);
        }

        [TestMethod]
        public async Task Post_RegisterVlaid_ReturnsSuccessfulResult()
        {
            //arrange
            var registerModel = new RegisterModel { Email = "test@test.com", Password = "test123" };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                             .ReturnsAsync(IdentityResult.Success);

            // act
            var result = await _accountController.Post(registerModel);

            //assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var registerResultValue = ((OkObjectResult)result).Value as RegisterResult;

            Assert.IsNotNull(registerResultValue);
            Assert.IsTrue(registerResultValue.Successful);
            Assert.IsNull(registerResultValue.Errors);

            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task Post_RegisterInvalid_ReturnsErrorResult()
        {
            //arrange
            var registerModel = new RegisterModel { Email = "test@test.com", Password = "test123" };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                             .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak" }));

            //act
            var result = await _accountController.Post(registerModel);

            //assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var registerResultValue = ((OkObjectResult)result).Value as RegisterResult;

            Assert.IsNotNull(registerResultValue);
            Assert.IsFalse(registerResultValue.Successful);
            Assert.IsNotNull(registerResultValue.Errors);
            Assert.AreEqual(1, registerResultValue.Errors.Count());
            Assert.AreEqual("Password is too weak", registerResultValue.Errors.First());

            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        }
    }
}