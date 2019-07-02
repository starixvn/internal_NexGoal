import React, { Component, Fragment } from 'react';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import moment from 'moment';

import getResource from '../../../resourceHelper';
import { PerformanceProcess, DateFormat } from '../../../constants';
import { noDataIndication } from '../../../common-functions';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    renderProbationer = (probationer, index) => {
        var evaluatorProcess = probationer.settings.find(p => p.processId === PerformanceProcess.Evaluator);
        var reviewerProcess = probationer.settings.find(p => p.processId === PerformanceProcess.Reviewer);
        var selfProcess = probationer.settings.find(p => p.processId === PerformanceProcess.Self);

        return (
            <tr key={probationer.employeeId}>
                <td className="text-center">{index + 1}</td>
                <td>
                    <div>{probationer.nickName} ({probationer.fullName})</div>
                    <div>
                        {selfProcess.fromDate && moment(selfProcess.fromDate).format('DD MMM, YYYY')}
                        {selfProcess.toDate && ` - ${moment(selfProcess.toDate).format('DD MMM, YYYY')}`}
                    </div>
                </td>
                <td>
                    <div>{evaluatorProcess.evaluatorNickName}</div>
                    <div>
                        {evaluatorProcess.fromDate && moment(evaluatorProcess.fromDate).format('DD MMM, YYYY')}
                        {evaluatorProcess.toDate && ` - ${moment(evaluatorProcess.toDate).format('DD MMM, YYYY')}`}
                    </div>
                </td>
                <td>
                    <div>{reviewerProcess.evaluatorNickName}</div>
                    <div>
                        {reviewerProcess.fromDate && moment(reviewerProcess.fromDate).format('DD MMM, YYYY')}
                        {reviewerProcess.toDate && ` - ${moment(reviewerProcess.toDate).format('DD MMM, YYYY')}`}
                    </div>
                </td>
                <td className="text-center">
                    <a className="fa fa-pencil-square-o" title="Edit" data-employeeid={probationer.employeeId} onClick={this.props.onEditSetting} />
                </td>
            </tr>
        );
    }

    renderTableContent = () => {
        if (this.props.tableData) {
            return this.props.tableData.length > 0 ? this.props.tableData.map(this.renderProbationer) :
                <tr><td className="text-center" colSpan={5}>{noDataIndication()}</td></tr>;
        } else {
            return <Fragment />;
        }
    }

    render() {
        return (
            <Fragment>
                <table className="table table-bordered table-striped">
                    <thead>
                        <tr className="header-control">
                            <th colSpan="5">
                                <button onClick={this.props.onAddNewEmployee} className="pull-left btn btn-primary">Add New Employee(s)</button>
                            </th>
                        </tr>
                        <tr>
                            <th className="order">{getResource('SymbolCount')}</th>
                            <th className="nick-name ">{getResource('Employee')}</th>
                            <th className="job-title">{getResource('Evaluator')}</th>
                            <th className="job-title">{getResource('Reviewer')}</th>
                            <th className="action">{getResource('Action')}</th>
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