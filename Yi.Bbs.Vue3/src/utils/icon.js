export const getUrl = (fileId) => {
    if (fileId == null || fileId == undefined) {
        return "/acquiesce.png"
    }
    else {
        return getEnvUrl(fileId)
    }

};

const getEnvUrl = (str) => {
    return `${import.meta.env.VITE_APP_BASEAPI}/file/${str}`;
};


