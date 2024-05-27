import React, { Component, Fragment } from 'react';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import getResource from '../../../resourceHelper';
import { PerformanceProcess, EmptyPlaceHolder } from '../../../constants';
import { noDataIndication } from '../../../common-functions';

export default class Table extends Component {
    constructor(props) {
        super(props);
        this.state = {
            localNoPR: props.noPR
        };
    }

    getOverlay = details => {
        if (details.length > 0) {
            return (<Tooltip id="tooltip">{details.map(d => <p className="creator-item" key={d.creatorId}>{d.creatorNickname}: {d.quantity}</p>)}</Tooltip>);
        }

        return <Fragment />;
    };

    updateChildNoPR = (newNoPR) => {
        this.setState({ NoPR: newNoPR });
    };

    renderEmployee = (employee, index) => {
        const { nickName, fullName, jobTitle, selfPRQuantity, details } = employee;

        const noPR = this.state.NoPR;
        const renderPRQuantity = (roleId, quantity) => {
            const filteredDetails = details.filter(d => d.roleId === roleId);
            const noPRValue = noPR && noPR.value;
    
            if (noPRValue === roleId && filteredDetails.length > quantity) {
                return filteredDetails.filter(d => d.quantity === 0).map(detail => detail.creatorNickname).join(', ');
            } else {
                return quantity > 0 ? quantity : EmptyPlaceHolder;
            }
        };
    
        return (
            <tr key={employee.employeeId}>
                <td className="text-center">{index + 1}</td>
                <td>{`${nickName} (${fullName})`}</td>
                <td>{jobTitle}</td>
                <td className="text-center">{selfPRQuantity > 0 ? selfPRQuantity : EmptyPlaceHolder}</td>
                <td className="text-center">
                    <OverlayTrigger placement="bottom" overlay={this.getOverlay(details.filter(d => d.roleId === PerformanceProcess.ProjectManager))}>
                        <span className="pr-quantity">{renderPRQuantity(PerformanceProcess.ProjectManager, employee.pmPRQuantity)}</span>
                    </OverlayTrigger>
                </td>
                <td className="text-center">
                    <OverlayTrigger placement="bottom" overlay={this.getOverlay(details.filter(d => d.roleId === PerformanceProcess.Evaluator))}>
                        <span className="pr-quantity">{renderPRQuantity(PerformanceProcess.Evaluator, employee.evaluatorPRQuantity)}</span>
                    </OverlayTrigger>
                </td>
                <td className="text-center">
                    <OverlayTrigger placement="bottom" overlay={this.getOverlay(details.filter(d => d.roleId === PerformanceProcess.Reviewer))}>
                        <span className="pr-quantity">{renderPRQuantity(PerformanceProcess.Reviewer, employee.reviewerPRQuantity)}</span>
                    </OverlayTrigger>
                </td>
            </tr>
        );
    };

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