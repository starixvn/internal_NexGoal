import React, { Component } from 'react';

import getResource from '../../../resourceHelper';

class EmployeeFlowSetting extends Component {
    render() {
        let employeeFlowSettings = this.props.employeeFlowSettings;

        return (
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th>{getResource('SymbolCount')}</th>
                        <th>{getResource('Code')}</th>
                        <th>{getResource('NickName')}</th>
                        <th>{getResource('FullName')}</th>
                        <th>{getResource('Team')}</th>
                        <th>{getResource('Admin')}</th>
                        <th>{getResource('Evaluator')}</th>
                        <th>{getResource('Reviewer')}</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        employeeFlowSettings.length === 0 &&
                        <tr>
                            <td className="empty-result" colSpan="8">{getResource('NoInformationAvailable')}</td>
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