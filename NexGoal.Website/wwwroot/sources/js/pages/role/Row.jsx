import React, { Component } from 'react';

class Row extends Component {
    constructor(props) {
        super(props);

        this.state = {
            policy: 0
        };
    }

    componentWillMount() {
        let policy = this.props.function.policy;

        this.setState({ policy });
    }

    componentDidUpdate(prevProps, prevState) {
        let policy = this.props.function.policy;

        if (policy !== prevState.policy) {
            this.setState({ policy });
        }
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

    handleChange(event) {
        let name = event.target.name;
        let policy = 0;
        if (event.target.checked) {
            policy = name === 'funcRead' ? 1 : 2;
        } else {
            policy = name === 'funcRead' ? 0 : this.state.policy === 2 ? 1 : 0;
        }

        this.setState({ policy });
        this.props.function.policy = policy;
        this.props.handleCheckboxChange(this.props.function);
    }

    render() {
        return (
            <tr className={this.props.function.level === 0 ? "row-special" : ""}>
                {this.renderCellFunctionName(this.props.function)}

                <td className="text-center">
                    <label>
                        <input type="checkbox" name="funcRead" disabled={!this.props.isHost} checked={this.state.policy > 0}
                            onChange={this.handleChange.bind(this)} />
                    </label>
                </td>
                <td className="text-center">
                    <label>
                        <input type="checkbox" name="funcHost" disabled={!this.props.isHost} checked={this.state.policy === 2}
                            onChange={this.handleChange.bind(this)} />
                    </label>
                </td>
            </tr>
        );
    }
}

export default Row;