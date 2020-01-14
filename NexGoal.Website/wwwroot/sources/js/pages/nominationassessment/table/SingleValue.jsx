import React, { PureComponent } from 'react';
import { components } from 'react-select';

import { getNominationClassName } from '../../../common-functions';

export default class SingleValue extends PureComponent {
    render() {
        return (
            <components.SingleValue {...this.props}>
                <span className={getNominationClassName(this.props.selectProps.value.value)}>{this.props.children}</span>
            </components.SingleValue>
        );
    }
}