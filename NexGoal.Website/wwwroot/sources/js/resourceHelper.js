const DEFAULT_RESOURCE_NAME = "resource_en-US";
const langCode = window.PageLanguage || "en-US";
const resourceName = `resource_${langCode.replace("-", "_")}`;

const getResource = (key) => {
    const currentResource = window[resourceName];

    return currentResource[key] || window[DEFAULT_RESOURCE_NAME][key] || key;
};

export default getResource;