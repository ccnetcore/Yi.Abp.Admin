import {listData} from "@/api/code/tableApi.js"

const useTable=()=>{
    const dataList=ref([]);
    onMounted( async() => {
    const response= await listData();
    dataList.value=response.data.items;
      });
    return {dataList};
}
export default useTable;