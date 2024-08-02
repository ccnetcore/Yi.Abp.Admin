export const getUrl = (fileId) => {
    if (fileId == null || fileId == undefined) {
        return "/acquiesce.png"
    }
    if (fileId.startsWith(`${import.meta.env.VITE_APP_BASEAPI}`)) {
        return fileId;

    }


    return getEnvUrl(fileId)

};

const getEnvUrl = (str) => {
    return `${import.meta.env.VITE_APP_BASEAPI}/file/${str}`;
};


