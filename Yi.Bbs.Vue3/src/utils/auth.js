import useUserStore from "@/stores/user";
const TokenKey = "Admin-Token";

export function getToken() {
  return localStorage.getItem(TokenKey);
}

export function setToken(token) {
  return localStorage.setItem(TokenKey, token);
}

export function removeToken() {
  return localStorage.removeItem(TokenKey);
}

export function getPermission(code, isDisabled = false) {
  const all_permission = "*:*:*";
  const isHasPermission = useUserStore().permissions.some((permission) => {
    if (all_permission === permission) {
      return true;
    } else {
      return code.includes(permission) && !isDisabled;
    }
  });
  return {
    isHasPermission,
  };
}
