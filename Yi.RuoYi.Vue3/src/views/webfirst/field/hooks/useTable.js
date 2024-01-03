import {listData} from "@/api/webfirst/tableApi.js"

const useTable=()=>{
    const dataList=ref([]);
    onMounted( async() => {
    const response= await listData();
    dataList.value=response.data.items;
      });
    return {dataList};
}
export default useTable;