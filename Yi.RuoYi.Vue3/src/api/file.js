import request from '@/utils/request'

export function
   upload(data){
    return request({
        url: `/file`,
        headers:{"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"},
        method: 'POST',
        data:data
      });
} 
