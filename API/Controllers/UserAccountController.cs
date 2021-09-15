using System.Threading.Tasks;
using API.DTOs;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserAccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        public UserAccountController(UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IJwtService jwtService, 
            IMapper mapper)
        {
            _mapper = mapper;
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            return new UserDTO
            {
                Email = user.Email,
                Token = _jwtService.CreateToken(user),
                Nickname = user.Nickname
            };
        }

        [HttpGet("emailcheck")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<UserAddressDTO>> GetUserAddress()
        {
            var user = await _userManager.FindByUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

            return _mapper.Map<UserAddress, UserAddressDTO>(user.UserAddress);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<UserAddressDTO>> UpdateUserAddress(UserAddressDTO address)
        {
            var user = await _userManager.FindByUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

            user.UserAddress = _mapper.Map<UserAddressDTO, UserAddress>(address);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return Ok(_mapper.Map<UserAddress, UserAddressDTO>(user.UserAddress));

            return BadRequest("Problem updating the user");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null) return Unauthorized(new ApiErrorResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if (!result.Succeeded) return Unauthorized(new ApiErrorResponse(401));

            return new UserDTO
            {
                Email = user.Email,
                Token = _jwtService.CreateToken(user),
                Nickname = user.Nickname
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegistrationDTO registerDTO)
        {
            if (CheckEmailExistsAsync(registerDTO.Email).Result.Value)
            {
                return new BadRequestObjectResult(
                    new ApiValidationErrorResponse{Errors = new []{"Email address is already in use!"}});
            }
            var user = new User
            {
                Nickname = registerDTO.Nickname,
                Email = registerDTO.Email,
                UserName = registerDTO.Email
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded) return BadRequest(new ApiErrorResponse(400));

            return new UserDTO
            {
                Nickname = user.Nickname,
                Token = _jwtService.CreateToken(user),
                Email = user.Email
            };
        }
    }
}