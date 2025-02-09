import { login, logout, register,retrievePassword } from "@/apis/accountApi";
import { getUserDetailInfo, getLoginCode } from "@/apis/auth";
import useAuths from "@/hooks/useAuths";
import { defineStore } from "pinia";
import { getBbsUserProfile } from '@/apis/userApi.js'
const { getToken, setToken, clearStorage } = useAuths();

const useUserStore = defineStore("user", {
  state: () => ({
    id: "",
    token: getToken(),
    name: "游客",
    userName: "",
    icon: null,
    roles: [],
    permissions: [],
    codeImageURL: "",
    codeUUid: "",
    money:0
  }),
  getters: {
      moneyFixed: (state) => state.money.toFixed(2)
  },
  actions: {
    // 登录
    login(userInfo) {
      const userName = userInfo.userName.trim();
      const password = userInfo.password;
      const code = userInfo.code;
      const uuid = userInfo.uuid;
      return new Promise((resolve, reject) => {
        login(userName, password, code, uuid)
          .then((response) => {
            const res = response.data;
            setToken(res.token);
   
            this.token = res.token;
            resolve(response);
          })
          .catch((error) => {
            reject(error);
          });
      });
    },
    // 获取用户信息
    getInfo() {
      return new Promise((resolve, reject) => {

        getUserDetailInfo()
          .then(async (response) => {
  
            const res = response.data;
            const user = res.user;
            const avatar =
              user.icon == "" || user.icon == null
                ? "/acquiesce.png"
                : import.meta.env.VITE_APP_BASEAPI + "/file/" + user.icon;
            if (res.roleCodes && res.roleCodes.length > 0) {
              // 验证返回的roles是否是一个非空数组
              this.roles = res.roleCodes;
              this.permissions = res.permissionCodes;
              // this.roles = ["admin"];
              // this.permissions=["*:*:*"]
            } else {
              this.roles = ["ROLE_DEFAULT"];
            }
     
            // this.roles = ["admin"];
            // this.permissions=["*:*:*"]
            this.name = user.nick;
            this.icon = avatar;

            this.userName = user.userName;
            this.id = user.id;
       

          //获取bbs信息
         const  {data:bbsData}=  await  getBbsUserProfile(this.id)
             
     
            this.money= bbsData.money;
   
            resolve(res);
          })
          .catch((error) => {
            reject(error);
          });




      });
    },
    // 退出系统
    logOut() {
      return new Promise((resolve, reject) => {
        logout()
          .then(() => {
            this.token = "";
            this.roles = [];
            this.permissions = [];
            clearStorage();
            resolve();
          })
          .catch((error) => {
            reject(error);
          });
      });
    },
    // 注册
    register(userInfo) {
      const userName = userInfo.userName.trim();
      const password = userInfo.password.trim();
      const phone = userInfo.phone;
      const uuid = userInfo.uuid;
      const code = userInfo.code;
      return new Promise((resolve, reject) => {
        register(userName, password, phone, code, uuid)
          .then((response) => {
            resolve(response);
          })
          .catch((error) => {
            reject(error);
          });
      });
    },
    // 重置用户信息
    resetInfo() {
      this.roles = [];
      this.permissions = [];
      this.name = "未登录";
      this.icon = "/login.svg";
      this.userName = "";
      this.id = "";
    },
    async updateCodeImage() {
      const { data } = await getLoginCode();
      this.codeImageURL = "data:image/jpg;base64," + data.img;
      this.codeUUid = data.uuid;
    },
    updateToken(token) {
      this.token = token;
    },
    //更新钱钱
    updateMoney(updateMoneyNumber){
        this.money=this.money+updateMoneyNumber
    }
  },
  persist: {
    key: "userInfo",
    storage: window.sessionStorage,
  },
});
export default useUserStore;
