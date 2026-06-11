using Microsoft.AspNetCore.Identity;
using OSM.Application.Abstractions.Authentication;
using OSM.Application.Abstractions.Services;
using OSM.Application.Features.BaseSetup.UserSetup;
using OSM.Infrastructure.Identity;

namespace OSM.Infrastructure.Services
{
    public sealed class UserService(UserManager<ApplicationUser> userManager , ICurrentUserService currentUser) : IUserService
    {
        public async Task<bool> DeleteUserAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            var deleteResult = await userManager.DeleteAsync(user);
            return deleteResult.Succeeded;
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserResponse> UpdateUserAsync(UpdateUserRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return null;
            }
            else
            {
                // ==========================================
                // 1. XỬ LÝ ĐỔI MẬT KHẨU (NẾU CÓ THAY ĐỔI)
                // ==========================================
                if (user.PasswordShow != request.Password)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var changePasswordResult = await userManager.ResetPasswordAsync(user, token, request.Password);

                    if (!changePasswordResult.Succeeded)
                    {
                        // Nếu đổi mật khẩu thất bại (ví dụ: mật khẩu mới không đủ độ phức tạp), dừng lại luôn
                        return null;
                    }
                }

                // ==========================================
                // 2. XỬ LÝ CẬP NHẬT ROLE (CHỈ ĐỔI KHI THỰC SỰ KHÁC)
                // ==========================================
                var currentRoles = await userManager.GetRolesAsync(user);

                if (currentRoles.Count != 1 || !currentRoles.Contains(request.Role))
                {
                    var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded) return null;

                    var addResult = await userManager.AddToRoleAsync(user, request.Role);
                    if (!addResult.Succeeded) return null;
                }

                // ==========================================
                // 3. CẬP NHẬT THÔNG TIN PROFILE CƠ BẢN
                // ==========================================
                user.FullName = request.FullName;
                user.Email = request.Email;
                user.PasswordShow = request.Password;
                user.IsActive = request.IsActive;
                user.Department = request.Department;
                user.ModifiedAt = DateTimeOffset.UtcNow;
                user.ModifiedBy = currentUser.UserName;

                var updateResult = await userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    return null;
                }

                UserResponse response = new(

                     user.UserName,
                     user.FullName,
                     user.FullName,
                     user.Email,
                     user.PasswordShow,
                     request.Role,
                     user.Department,
                     user.IsActive,
                     user.CreatedAt,
                     user.CreatedBy,
                     user.ModifiedAt,
                     user.ModifiedBy
                );

                return response;
            }
        }
    }
}
