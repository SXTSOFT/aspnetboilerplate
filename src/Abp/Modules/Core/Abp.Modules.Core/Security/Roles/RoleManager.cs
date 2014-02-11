﻿using System.Collections.Concurrent;
using Abp.Dependency;
using Abp.Utils.Extensions.Collections;

namespace Abp.Security.Roles
{
    public class RoleManager : IRoleManager
    {
        private readonly IAbpRoleRepository _roleRepository;

        private readonly ConcurrentDictionary<string, RoleInfo> _roles;
        
        public RoleManager(IAbpRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
            _roles = new ConcurrentDictionary<string, RoleInfo>();
            Initialize();
        }
        
        public RolePermission GetPermissionOrNull(string roleName, string permissionName)
        {
            RoleInfo roleInfo;
            if (!_roles.TryGetValue(roleName, out roleInfo))
            {
                //No role with given name, so there is no permission setting!
                return null;
            }

            return roleInfo.Permissions.GetOrDefault(permissionName);
        }

        private void Initialize()
        {
            var roles = _roleRepository.GetAllListWithPermissions();
            foreach (var role in roles)
            {
                _roles[role.Name] = new RoleInfo(role);
            }
        }

        private class RoleInfo
        {
            public AbpRole Role { get; private set; }

            public ConcurrentDictionary<string, RolePermission> Permissions { get; private set; }

            public RoleInfo(AbpRole role)
            {
                Role = role;
                Permissions = new ConcurrentDictionary<string, RolePermission>();
                foreach (var permission in role.Permissions)
                {
                    Permissions[permission.PermissionName] = permission;
                }
            }
        }
    }
}