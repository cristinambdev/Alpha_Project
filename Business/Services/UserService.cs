using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Business.Services;

public interface IUserService
{
    Task<UserResult> AddUserToRole(string userId, string roleName);
    Task<UserResult> CreateUserAsync(AddUserFormData formData, string roleName = "User");
    Task<UserResult> GetUsersAsync();
    Task<UserResult> UpdateUserAsync(EditUserFormData formData);
    Task<UserResult> GetUserByIdAsync(string id);
    
    }

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;


    public async Task<UserResult> GetUsersAsync()
    {
        var result = await _userRepository.GetAllAsync();
        return result.MapTo<UserResult>(); // dynamic mapping

        //var list = await _userManager.Users.ToListAsync();
        //return list.MapTo<UserResult>();
    }

    public async Task<UserResult> GetUserByIdAsync(string id)
    {
        var result = await _userRepository.GetAsync(x => x.Id == id);
        return result.MapTo<UserResult>();
    }
    public async Task<UserResult> AddUserToRole(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "Role doesn't exist." };

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "User doesn't exist." };


        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded
            ? new UserResult { Succeeded = true, StatusCode = 200 }
            : new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to add user to role." };
    }


    public async Task<UserResult> CreateUserAsync(AddUserFormData formData, string roleName = "User")
    {


        if (formData == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "Form data can't be null" };

        var existsResult = await _userRepository.ExistsAsync(x => x.Email == formData.Email);
        if(existsResult.Succeeded)
            return new UserResult { Succeeded = false, StatusCode = 409, Error = "user with same email already exists." };


        try
        {
            //var userEntity = formData.MapTo<UserEntity>();

            var userEntity = new UserEntity // suggested by Chat Gpt as the dynamic mapping didn't wok 
            {
                FirstName = formData.FirstName,
                LastName = formData.LastName,
                Email = formData.Email,
                UserName = formData.Email,
                PhoneNumber = formData.PhoneNumber,   
                UserImage = formData.UserImage
            };
            var defaultPassword = "ChangeMe123!"; //suggested by Chat Gpt
            var result = await _userManager.CreateAsync(userEntity, defaultPassword);
            //var result = await _userRepository.AddAsync(userEntity);
            if (result.Succeeded)
            {
                var addToRoleResult = await AddUserToRole(userEntity.Id, roleName);
                return addToRoleResult.Succeeded
                ? new UserResult { Succeeded = true, StatusCode = 201, }
                : new UserResult { Succeeded = false, StatusCode = 500, Error = "User created but not added to role." };

            }

            return new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }

    }

    public async Task<UserResult> UpdateUserAsync(EditUserFormData formData)
    {
        var userEntity = await _userManager.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == formData.Id);
        if (userEntity == null)
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "User not found" };
        //userEntity = formData.MapTo<UserEntity>();

        // Update fields on the existing entity
        userEntity.FirstName = formData.FirstName;
        userEntity.LastName = formData.LastName;
        userEntity.Email = formData.Email;
        userEntity.UserName = formData.Email;
        userEntity.PhoneNumber = formData.PhoneNumber;
        userEntity.JobTitle = formData.JobTitle;
        userEntity.UserImage = formData.UserImage;

        // Update or create address
        userEntity.Address = new UserAddressEntity();

        userEntity.Address.StreetName = formData.StreetName!;
        userEntity.Address.PostalCode = formData.PostalCode!;
        userEntity.Address.City = formData.City!;

        var result = await _userManager.UpdateAsync(userEntity);
        //var result = await _userRepository.UpdateAsync(userEntity);

        return new UserResult { Succeeded = true, StatusCode = 200};
    }

}


