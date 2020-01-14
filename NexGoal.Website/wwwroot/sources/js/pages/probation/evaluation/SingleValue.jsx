import React, { PureComponent }  from 'react';
import { components } from 'react-select';

export default class SingleValue extends PureComponent {
    render() {
        return (
            <components.SingleValue {...this.props}>
                <span className={this.props.selectProps.value.value ? 'passed' : 'failed'} >{this.props.children}</span>
            </components.SingleValue>
        );
    }
}