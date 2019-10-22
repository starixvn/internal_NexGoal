import React from 'react';
import { components } from 'react-select';

const MultiValueContainer = props => {
    const selectedItems = props.selectProps.value;
    const totalItems = props.selectProps.value.length;
    const currentItem = props.data;
    const index = selectedItems.indexOf(currentItem);

    const containerProps = { ...props };
    const innerProps = { ...containerProps.innerProps };
    innerProps.className = index > 0 ? innerProps.className + ' hide' : innerProps.className;
    containerProps.innerProps = { ...containerProps.innerProps, ...innerProps };

    return (
        <components.MultiValueContainer {...containerProps}>
            {`${totalItems} item(s) selected`}
        </components.MultiValueContainer>
    );
};

export default MultiValueContainer;