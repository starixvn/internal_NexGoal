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
                    <span className="icon-root_tree icon-level-second"></span>
                    <span className="func-name">{func.name}</span>
                </td>
            );
        }

        return (
            <td className="level-first">
                <span className="icon-ui icon-level-first"></span>
                <span className="func-name">{func.name}</span>
            </td>
        );
    }

    handleOnChange(event) {
        let name = event.target.name;
        let policy = 0;
        if (event.target.checked) {
            policy = name === 'funcRead' ? 1 : 2;
        } else {
            policy = name === 'funcRead' ? 0 : this.props.function.policy === 2 ? 1 : 0;
        }

        this.props.function.policy = policy;
        this.props.onCheckboxChange(this.props.function);
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