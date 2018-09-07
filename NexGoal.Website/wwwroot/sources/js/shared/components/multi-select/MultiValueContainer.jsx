import React from 'react';
import { components } from 'react-select';

const MultiValueContainer = (props) => {
    let selectedItems = props.selectProps.value;
    let totalItems = props.selectProps.value.length;
    let currentItem = props.data;
    let index = selectedItems.indexOf(currentItem);

    let containerProps = { ...props };
    let innerProps = { ...containerProps.innerProps };
    innerProps.className = index > 0 ? innerProps.className + ' hide' : innerProps.className;
    containerProps.innerProps = { ...containerProps.innerProps, ...innerProps };

    return (
        <components.MultiValueContainer {...containerProps}>
            {`${totalItems} item(s) selected`}
        </components.MultiValueContainer>
    );
};

export default MultiValueContainer;