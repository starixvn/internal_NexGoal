import React from 'react';
import { components } from 'react-select';

import { getNominationClassName } from '../../../common-functions';

const SingleValue = ({ children, ...props }) => {
    return (
        <components.SingleValue {...props}>
            <span className={getNominationClassName(props.selectProps.value.value)} >{children}</span>
        </components.SingleValue>
    );
};

export default SingleValue;