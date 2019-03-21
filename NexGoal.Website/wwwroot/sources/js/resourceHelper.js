const getResource = key => {
    const currentResource = window['resource_en_US'];

    return currentResource ? currentResource[key] : key;
};

export default getResource;