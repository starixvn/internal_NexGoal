import React, { Component } from 'react';

import { Policy } from '../../../constants';

export default class Row extends Component {
    renderCellFunctionName(func) {
        if (func.parentId) {
            return (
                <td>
                    <span className="icon-treedot" />
                    <span className="func-name">{func.name}</span>
                </td>
            );
        }

        return (
            <td className="level-first">
                <span className="icon-menu-down" />
                <span className="func-name">{func.name}</span>
            </td>
        );
    }

    handleOnChange = (event) => {
        this.props.onCheckboxChange(this.props.function, event.target.name, event.target.checked);
    }

    render() {
        const functionObj = this.props.function;

        return (
            <tr className={!functionObj.parentId ? "row-special" : ""}>
                {this.renderCellFunctionName(functionObj)}

                <td className="text-center">
                    {
                        (functionObj.onlyPolicy === null || functionObj.onlyPolicy === Policy.Read) &&
                        <div className="checkbox">
                            <input type="checkbox" name="funcRead" className="checkbox-custom"
                                disabled={!this.props.isHost}
                                checked={functionObj.policy > Policy.None}
                                onChange={this.handleOnChange} id={"chb-read-" + functionObj.functionId}
                            />
                            <label htmlFor={"chb-read-" + functionObj.functionId} />
                        </div>
                    }
                </td>
                <td className="text-center">
                    {
                        (functionObj.onlyPolicy === null || functionObj.onlyPolicy === Policy.Host) &&
                        <div className="checkbox">
                            <input type="checkbox" name="funcHost" className="checkbox-custom"
                                disabled={!this.props.isHost}
                                checked={functionObj.policy === Policy.Host}
                                onChange={this.handleOnChange} id={"chb-host-" + functionObj.functionId}
                            />
                            <label htmlFor={"chb-host-" + functionObj.functionId} />
                        </div>
                    }
                </td>
            </tr>
        );
    }
}