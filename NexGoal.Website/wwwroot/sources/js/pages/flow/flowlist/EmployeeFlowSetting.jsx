import React, { Component } from 'react';

class EmployeeFlowSetting extends Component {
    render() {
        let employeeFlowSettings = this.props.employeeFlowSettings;

        return (
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Code</th>
                        <th>Nick Name</th>
                        <th>Full Name</th>
                        <th>Team</th>
                        <th>Admin</th>
                        <th>Evaluator</th>
                        <th>Reviewer</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        employeeFlowSettings.length === 0 &&
                        <tr>
                            <td className="empty-result" colSpan="8">No information available!</td>
                        </tr>
                    }
                    {
                        employeeFlowSettings.length > 0 &&
                        employeeFlowSettings.map((employeeFlowSetting, index) => {
                            return (
                                <tr key={`employeeFlowSetting${employeeFlowSetting.groupId}`}>
                                    <td className="text-center">{index + 1}</td>
                                    <td className="text-center">{employeeFlowSetting.code}</td>
                                    <td className="text-center">{employeeFlowSetting.nickName}</td>
                                    <td>{employeeFlowSetting.fullName}</td>
                                    <td>{employeeFlowSetting.groupName}</td>
                                    <td>
                                        {
                                            employeeFlowSetting.humanResources.length > 0 &&
                                            employeeFlowSetting.humanResources.map(humanResource => {
                                                return (
                                                    <p key={`humanResource${humanResource.evaluatorId}`}>
                                                        {`${humanResource.fullName} (${humanResource.nickName})`}
                                                    </p>
                                                );
                                            })
                                        }
                                    </td>
                                    <td>
                                        {
                                            employeeFlowSetting.evaluator ?
                                                `${employeeFlowSetting.evaluator.fullName} (${employeeFlowSetting.evaluator.nickName})`
                                                : ''
                                        }
                                    </td>
                                    <td>
                                        {
                                            employeeFlowSetting.reviewer ?
                                                `${employeeFlowSetting.reviewer.fullName} (${employeeFlowSetting.reviewer.nickName})`
                                                : ''
                                        }</td>
                                </tr>
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
}

export default EmployeeFlowSetting;