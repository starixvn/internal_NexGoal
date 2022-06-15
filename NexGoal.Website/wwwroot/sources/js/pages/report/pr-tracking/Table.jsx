import React, { Component, Fragment } from 'react';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';

import getResource from '../../../resourceHelper';
import StickyBlock from '../../../shared/components/StickyBlock';
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
                    <td colSpan={7}>{data.groupName}</td>
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
                            <td colSpan={7}>{this.props.period.name}</td>
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

    renderPRTrackingStickyTable = () => {
       return (
            <table className="table table-bordered table-striped pr-tracking-table" id="pr-tracking-sticky">
                <thead>
                    <tr>
                        <th rowSpan="2" className="order" >{getResource('SymbolCount')}</th>
                        <th className="nick-name " rowSpan="2">{getResource('Employee')}</th>
                        <th rowSpan="2" className="job-title">{getResource('JobTitle')}</th>
                        <th colSpan="4">Performance Record Quantity</th>
                    </tr>
                    <tr>
                        <th className="stick-block self">{getResource('Self')}</th>
                        <th className="stick-block pm">{getResource('ProjectManager')}</th>
                        <th className="evaluator stick-block">{getResource('Evaluator')}</th>
                        <th className="reviewer stick-block">{getResource('Reviewer')}</th>
                    </tr>
                </thead>
            </table>

        );
    };

    render() {
        return (
            <Fragment>
                <StickyBlock threshold={236}>
                {
                        this.renderPRTrackingStickyTable()
                }
                </StickyBlock>
                <table id="pr-tracking-table" className="table table-bordered pr-tracking-table table-striped">
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
            </Fragment>
        );
    }
}