import React from 'react';
import { components } from 'react-select';

const SingleValue = ({ children, ...props }) => {
    return (
        <components.SingleValue {...props}>
            <span className={props.selectProps.value.value ? 'passed' : 'failed'} >{children}</span>
        </components.SingleValue>
    );
};

export default SingleValue;