import React, { Component } from 'react';

export default class ConfiguredEmployeeList extends Component {
    renderEmployee = employee =>
        (
            <div key={employee.employeeId} onClick={this.props.onClick.bind(this, employee)}
                className={this.props.selectedEmployeeId === employee.employeeId ? 'selected' : ''}
            >
                {employee.nickName} ({employee.fullName})
            </div>
        )

    render() {
        return (
            <div className="employee-list">
                <div className="header">Already Configured</div>
                <div className="content">
                    {this.props.employees.map(this.renderEmployee)}
                </div>
            </div>
        );
    }
}