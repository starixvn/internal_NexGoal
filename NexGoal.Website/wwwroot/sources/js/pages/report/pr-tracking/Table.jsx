import React, { Component, Fragment } from 'react';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';

import getResource from '../../../resourceHelper';
import StickyBlock from '../../../shared/components/StickyBlock';
import { PerformanceProcess } from '../../../constants';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    getOverlay = (details) => {
        if (details.length > 0) {
            return (<Tooltip id="tooltip">{details.map(d => <p key={d.creatorId}>{d.creatorNickname}: {d.quantity}</p>)}</Tooltip>);
        }

        return <Fragment />;
    }

    renderEmployee = (employee, index) => (
        <tr key={employee.employeeId}>
            <td className="text-center">{index + 1}</td>
            <td>{employee.nickName} ({employee.fullName})</td>
            <td>{employee.jobTitle}</td>
            <td className="text-center">{employee.selfPRQuantity > 0 ? employee.selfPRQuantity : '-'}</td>
            <OverlayTrigger placement="bottom" overlay={this.getOverlay(employee.details.filter(d => d.roleId === PerformanceProcess.ProjectManager))}>
                <td className="text-center">
                    {employee.pmPRQuantity > 0 ? employee.pmPRQuantity : '-'}
                </td>
            </OverlayTrigger>
            <OverlayTrigger placement="bottom" overlay={this.getOverlay(employee.details.filter(d => d.roleId === PerformanceProcess.Evaluator))}>
                <td className="text-center">
                    {employee.evaluatorPRQuantity > 0 ? employee.evaluatorPRQuantity : '-'}
                </td>
            </OverlayTrigger>
            <OverlayTrigger placement="bottom" overlay={this.getOverlay(employee.details.filter(d => d.roleId === PerformanceProcess.Reviewer))}>
                <td className="text-center">
                    {employee.reviewerPRQuantity > 0 ? employee.reviewerPRQuantity : '-'}
                </td>
            </OverlayTrigger>
        </tr>
    )

    renderData = data => {
        return (
            <Fragment key={data.groupId}>
                <tr className="team-group-row">
                    <td colSpan={7}>{data.groupName}</td>
                </tr>
                {data.employees.map(this.renderEmployee)}
            </Fragment >
        );
    }

    renderTableContent = () => {
        if (this.props.tableData && this.props.period) {
            return (
                <Fragment>
                    <tr className="group-row">
                        <td colSpan={7}>{this.props.period.name}</td>
                    </tr>
                    {this.props.tableData.map(this.renderData)}
                </Fragment>);
        } else {
            return <Fragment />;
        }
    }

    render() {
        return (
            <Fragment>
                <StickyBlock threshold={236}>
                    <table className="table table-bordered table-striped pr-tracking-table">
                        <thead>
                            <tr>
                                <th className="order" rowSpan="2">{getResource('SymbolCount')}</th>
                                <th className="nick-name " rowSpan="2">{getResource('Employee')}</th>
                                <th className="job-title" rowSpan="2">{getResource('JobTitle')}</th>
                                <th colSpan="4">Performance Record Quantity</th>
                            </tr>
                            <tr>
                                <th className="self">{getResource('Self')}</th>
                                <th className="pm">{getResource('ProjectManager')}</th>
                                <th className="evaluator">{getResource('Evaluator')}</th>
                                <th className="reviewer">{getResource('Reviewer')}</th>
                            </tr>
                        </thead>
                    </table>
                </StickyBlock>
                <table className="table table-bordered table-striped pr-tracking-table">
                    <thead>
                        <tr>
                            <th className="order" rowSpan="2">{getResource('SymbolCount')}</th>
                            <th className="nick-name " rowSpan="2">{getResource('Employee')}</th>
                            <th className="job-title" rowSpan="2">{getResource('JobTitle')}</th>
                            <th colSpan="4">Performance Record Quantity</th>
                        </tr>
                        <tr>
                            <th className="self">{getResource('Self')}</th>
                            <th className="pm">{getResource('ProjectManager')}</th>
                            <th className="evaluator">{getResource('Evaluator')}</th>
                            <th className="reviewer">{getResource('Reviewer')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.renderTableContent()}
                    </tbody>
                </table>
            </Fragment>
        );
    }
}