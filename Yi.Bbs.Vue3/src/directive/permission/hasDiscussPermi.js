 /**
 * v-hasDiscussPermi 操作权限处理
 */
 
 import useUserStore from '@/stores/user'

 //传一个值，一个主题id的创建者id，判断当前主题是否为自己创建，拥有*:*:*，直接跳过
 export default {
   mounted(el, binding, vnode) {
     const { value } = binding
     const all_permission = "*:*:*";
     const permissions = useUserStore().permissions
     const userId = useUserStore().id
     if (value && value instanceof Array && value.length > 0) {
       const permissionFlag = value
 
       const hasPermissions = permissions.some(permission => {
         return all_permission === permission || permissionFlag==userId
       })
 
       if (!hasPermissions) {
         el.parentNode && el.parentNode.removeChild(el)
       }
     } else {
       throw new Error(`请设置操作主题用户签值`)
     }
   }
 }
 