import React, { Component, Fragment } from 'react';
import { Tooltip, Overlay } from 'react-bootstrap';

import { PerformanceProcess } from '../../../constants';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import axios from 'axios';

export default class Row extends Component {
    constructor(props) {
        super(props);
        this.state = {
            showOverlay: false,
            overlayData: [],
            target: null
        };
    }

    showOverlay = (employeeId, processId, event) => {
        if (this.state.target === event.target) {
            this.setState({ showOverlay: true });
            return;
        }

        const target = event.target;
        axios.get(resolveClientUrl('PRTracking/GetDetail'), {
            params: {
                employeeId: employeeId,
                processId: processId
            }
        }).then(response => {
            this.setState({
                showOverlay: true,
                overlayData: response.data,
                target: target
            });
        }).catch(axiosError);
    }

    hideOverlay = () => this.setState({ showOverlay: false })

    render() {
        const employee = this.props.employee;
        return (
            <Fragment>
                <tr>
                    <td className="text-center">{this.props.index + 1}</td>
                    <td>{employee.nickName} ({employee.fullName})</td>
                    <td>{employee.jobTitle}</td>
                    <td className="text-center">{employee.selfPRs > 0 ? employee.selfPRs : '-'}</td>
                    <td className="text-center">
                        <span className="pr-quantity" onMouseEnter={this.showOverlay.bind(this, employee.employeeId, PerformanceProcess.ProjectManager)} onMouseLeave={this.hideOverlay}>
                            {employee.pmpRs > 0 ? employee.pmpRs : '-'}
                        </span>
                    </td>
                    <td className="text-center">
                        <span className="pr-quantity" onMouseEnter={this.showOverlay.bind(this, employee.employeeId, PerformanceProcess.Evaluator)} onMouseLeave={this.hideOverlay}>
                            {employee.evaluatorPRs > 0 ? employee.evaluatorPRs : '-'}
                        </span>
                    </td>
                    <td className="text-center">
                        <span className="pr-quantity" onMouseEnter={this.showOverlay.bind(this, employee.employeeId, PerformanceProcess.Reviewer)} onMouseLeave={this.hideOverlay}>
                            {employee.reviewerPRs > 0 ? employee.reviewerPRs : '-'}
                        </span>
                    </td>
                </tr>
                <Overlay target={this.state.target} show={this.state.showOverlay} placement="bottom">
                    <Tooltip id="tooltip">
                        {this.state.overlayData.map(e => <p key={e.employeeId}>{e.nickName}: {e.quantity}</p>)}
                    </Tooltip>
                </Overlay>
            </Fragment>
        );
    }
}