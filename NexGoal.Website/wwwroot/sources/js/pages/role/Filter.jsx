import React, { Component, Fragment } from 'react';

class Filter extends Component {
    constructor(props) {
        super(props);
    }

    handleChange(event) {
        this.props.handleRoleChange(event.target.value);
    }

    render() {
        const optionState = this.props.optionState;

        return (
            <form className="form-inline form-filter">
                <div className="form-group">
                    <div className="col">
                        <label className="control-label">Role</label>
                        <div className="control-content">
                            <select className="form-control" name="RoleId" value={optionState}
                                onChange={this.handleChange.bind(this)}>
                                {
                                    this.props.options.map((option) => {
                                        return (
                                            <option key={'option-' + option.value} value={option.value}>{option.text}</option>
                                        )
                                    })
                                }
                            </select>
                        </div>
                    </div>
                </div>
            </form>
        );
    }
}

export default Filter;