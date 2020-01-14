import React, { PureComponent } from 'react';
import { components } from 'react-select';

export default class Option extends PureComponent {
    render() {
        return (
            <components.Option {...this.props}>
                <div className="checkbox">
                    <input className="checkbox-custom" type="checkbox" checked={this.props.isSelected}
                        onChange={() => {
                            //empty
                        }}
                    />
                    <label>{this.props.data.label}</label>
                </div>
            </components.Option>
        );
    }
}