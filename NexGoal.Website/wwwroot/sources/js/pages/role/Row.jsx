import React, { Component } from 'react';

class Row extends Component {
    constructor(props) {
        super(props);

        this.handleOnChange = this.handleOnChange.bind(this);
    }

    renderCellFunctionName(func) {
        if (func.parentId) {
            return (
                <td>
                    <span className="icon-treedot"></span>
                    <span className="func-name">{func.name}</span>
                </td>
            );
        }

        return (
            <td className="level-first">
                <span className="icon-menu-down"></span>
                <span className="func-name">{func.name}</span>
            </td>
        );
    }

    handleOnChange(event) {
        this.props.onCheckboxChange(this.props.function, event.target.name, event.target.checked);
    }

    render() {
        const functionObj = this.props.function;

        return (
            <tr className={!functionObj.parentId ? "row-special" : ""}>
                {this.renderCellFunctionName(functionObj)}

                <td className="text-center">
                    <label>
                        <input type="checkbox" name="funcRead"
                            disabled={!this.props.isHost}
                            checked={functionObj.policy > 0}
                            onChange={this.handleOnChange} />
                    </label>
                </td>
                <td className="text-center">
                    <label>
                        <input type="checkbox" name="funcHost"
                            disabled={!this.props.isHost}
                            checked={functionObj.policy === 2}
                            onChange={this.handleOnChange} />
                    </label>
                </td>
            </tr>
        );
    }
}

export default Row;