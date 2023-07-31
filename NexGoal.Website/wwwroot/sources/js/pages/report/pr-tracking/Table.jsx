import React, { Component, Fragment } from 'react';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import getResource from '../../../resourceHelper';
import { PerformanceProcess, EmptyPlaceHolder } from '../../../constants';
import { noDataIndication } from '../../../common-functions';

export default class Table extends Component {
    getOverlay = details => {
        if (details.length > 0) {
            return (<Tooltip id="tooltip">{details.map(d => <p className="creator-item" key={d.creatorId}>{d.creatorNickname}: {d.quantity}</p>)}</Tooltip>);
        }

        return <Fragment />;
    };

    renderEmployee = (employee, index) => (
        <tr key={employee.employeeId}>
            <td className="text-center">{index + 1}</td>
            <td>{employee.nickName} ({employee.fullName})</td>
            <td>{employee.jobTitle}</td>
            <td className="text-center">{employee.selfPRQuantity > 0 ? employee.selfPRQuantity : EmptyPlaceHolder}</td>
            <td className="text-center">
                <OverlayTrigger placement="bottom" overlay={this.getOverlay(employee.details.filter(d => d.roleId === PerformanceProcess.ProjectManager))}>
                    <span className="pr-quantity">{employee.pmPRQuantity > 0 ? employee.pmPRQuantity : EmptyPlaceHolder}</span>
                </OverlayTrigger>
            </td>

            <td className="text-center">
                <OverlayTrigger placement="bottom" overlay={this.getOverlay(employee.details.filter(d => d.roleId === PerformanceProcess.Evaluator))}>
                    <span className="pr-quantity">{employee.evaluatorPRQuantity > 0 ? employee.evaluatorPRQuantity : EmptyPlaceHolder}</span>
                </OverlayTrigger>
            </td>
            <td className="text-center">
                <OverlayTrigger placement="bottom" overlay={this.getOverlay(employee.details.filter(d => d.roleId === PerformanceProcess.Reviewer))}>
                    <span className="pr-quantity">{employee.reviewerPRQuantity > 0 ? employee.reviewerPRQuantity : EmptyPlaceHolder}</span>
                </OverlayTrigger>
            </td>

        </tr>
    );

    renderData = data => {
        return (
            <Fragment key={data.groupId}>
                <tr className="team-group-row">
                    <td colSpan={2}><span>{data.groupName}</span></td>
                    <td colSpan={5} />
                </tr>
                {data.employees.map(this.renderEmployee)}
            </Fragment >
        );
    };

    renderPRTrackingTableContent = () => {
        if (this.props.tableData) {
            if (this.props.period) {
                return (
                    <Fragment>
                        <tr className="group-row">
                            <td colSpan={2}>{this.props.period.name}</td>
                            <td colSpan={5} />
                        </tr>
                        {this.props.tableData.length > 0 ? this.props.tableData.map(this.renderData) :
                            <tr><td className="text-center" colSpan={7}>{noDataIndication()}</td></tr>
                        }
                    </Fragment>);
            } else {
                return (<tr><td className="text-center" colSpan={7}>{noDataIndication()}</td></tr>);
            }
        } else {
            return <Fragment />;
        }
    };

    render() {
        return (
            <Fragment>
                <div className="pr-tracking-table ">
                    <table id="pr-tracking-table" className="table table-bordered table-striped">
                        <thead>
                            <tr>
                                <th className="order" rowSpan="2">{getResource('SymbolCount')}</th>
                                <th rowSpan="2" className="nick-name">{getResource('Employee')}</th>
                                <th className="job-title" rowSpan="2">{getResource('JobTitle')}</th>
                                <th colSpan="4">Performance Record Quantity</th>
                            </tr>
                            <tr>
                                <th className="self pr-tracking-header">{getResource('Self')}</th>
                                <th className="pm pr-tracking-header">{getResource('ProjectManager')}</th>
                                <th className="evaluator pr-tracking-header">{getResource('Evaluator')}</th>
                                <th className="reviewer pr-tracking-header">{getResource('Reviewer')}</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                this.renderPRTrackingTableContent()
                            }
                        </tbody>
                    </table>
                </div>
            </Fragment>
        );
    }
}