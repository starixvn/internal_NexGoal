import React, { Component, Fragment } from 'react';

import getResource from '../../../resourceHelper';

export default class Table extends Component {
    render() {
        const paFocuses = this.props.data;

        return (
            <table className="table table-bordered table-striped pa-focus-table">
                <thead>
                    <tr>
                        <th>{getResource('SymbolCount')}</th>
                        <th>{getResource('NickName')}</th>
                        <th>{getResource('FullName')}</th>
                        <th>{getResource('JobTitle')}</th>
                        <th>{getResource('ReadyForPromotion')}</th>
                        <th>{getResource('AccomplishedCurrentObjectives')}</th>
                        <th colSpan="2">{getResource('NextObjectives')}</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        paFocuses.length === 0 &&
                        <tr>
                            <td className="empty-result" colSpan="13">{getResource('NoInformationAvailable')}</td>
                        </tr>
                    }
                    {
                        paFocuses.length > 0 &&
                        paFocuses.map(record => {
                            return (
                                <Fragment key={`record${record.groupCode}`}>
                                    <tr className="group-row">
                                        <td colSpan="13">{record.groupName}</td>
                                    </tr>
                                    {
                                        record.members.map((member, index) => {
                                            return (
                                                <tr key={`member${member.paFocusId}`}>
                                                    <td className="text-center">{index + 1}</td>
                                                    <td>{member.employee.nickName}</td>
                                                    <td>{member.employee.fullName}</td>
                                                    <td>{member.employee.jobTitle}</td>
                                                    <td></td>
                                                    <td></td>
                                                    <td></td>
                                                </tr>
                                            );
                                        })
                                    }
                                </Fragment>
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
}