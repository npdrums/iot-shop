using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.IdentityDTOs;
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
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _JwtService;
        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IJwtService JwtService, 
            IMapper mapper)
        {
            _mapper = mapper;
            _JwtService = JwtService;
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
                Token = _JwtService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDTO>> GetUserAddress()
        {
            var user = await _userManager.FindByUserByClaimsPrincipleWithAddressAsync(HttpContext.User);

            return _mapper.Map<Address, AddressDTO>(user.Address);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDTO>> UpdateUserAddress(AddressDTO address)
        {
            var user = await _userManager.FindByUserByClaimsPrincipleWithAddressAsync(HttpContext.User);
            user.Address = _mapper.Map<AddressDTO, Address>(address);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) 
            {
                return Ok(_mapper.Map<Address, AddressDTO>(user.Address));
            }

            return BadRequest("Problem updating the user");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) 
            {
                return Unauthorized(new ApiErrorResponse(401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new ApiErrorResponse(401));
            }

            return new UserDTO
            {
                Email = user.Email,
                Token = _JwtService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegistrationDTO registerDto)
        {
            if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(
                    new ApiValidationErrorResponse 
                    {
                        Errors = new []{"Email address is in use"}
                    }
                );
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            { 
                return BadRequest(new ApiErrorResponse(400));
            }

            return new UserDTO
            {
                DisplayName = user.DisplayName,
                Token = _JwtService.CreateToken(user),
                Email = user.Email
            };
        }
    }
}