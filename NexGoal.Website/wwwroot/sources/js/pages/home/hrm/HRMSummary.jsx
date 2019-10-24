import React, { Fragment, Component } from 'react';
import moment from 'moment';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat } from '../../../constants';
import getResource from '../../../resourceHelper';
import Discipline from './DisciplineModal';

export default class HRMSummary extends Component {
    constructor(props) {
        super(props);
        this.state = {
            hrmDetail: {},
            isShowModal: false
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('Common/GetHRMSummary'), {
            params: {
                memberId: this.props.memberId ? this.props.memberId : null
            }
        }).then(response =>
            this.setState({
                hrmDetail: response.data
            })).catch(axiosError);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(nextState.hrmDetail) !== JSON.stringify(this.state.hrmDetail)
            || nextState.isShowModal !== this.state.isShowModal
            || nextProps.memberId !== this.props.memberId) {
            return true;
        }
        return false;
    }

    onShowModal = () => {
        this.setState({
            isShowModal: true
        });
    }

    onCloseModal = () => {
        this.setState({
            isShowModal: false
        });
    }

    renderToeicScore = hrmDetail => {
        let toeicClass = '';
        if (hrmDetail.toeicPolicy > 0) {
            toeicClass = 'toeic-passed';
        }

        if (hrmDetail.toeicPolicy < 0) {
            toeicClass = 'toeic-failed';
        }

        const expireDate = moment(hrmDetail.toeicExamDate).add(3, 'years').format(DateFormat);
        const showExpireDate = parseInt(hrmDetail.toeicScore, 10) < 900 ? `(exp ${expireDate})` : '';

        if (hrmDetail.toeicScore > 0) {
            return (
                <Fragment>
                    <span className={toeicClass}>{hrmDetail.toeicScore}</span> {showExpireDate}
                </Fragment>
            );
        } else {
            return (
                <span className={toeicClass}>{getResource('N/A')}</span>
            );
        }
    };

    renderDiscipline = incident => {
        if (incident > 0) {
            return <a onClick={this.onShowModal}>{incident}</a>;
        }

        return incident;
    }

    render() {
        const hrm = this.state.hrmDetail;
        const memberId = this.props.memberId ? this.props.memberId : null;
        return (
            <div className="hrm-summary box">
                <h5>{getResource('Hrm')}</h5>
                <ul className="hrm-info">
                    <li>{getResource('Toeic')}<span className="bold">: {this.renderToeicScore(hrm)}</span></li>
                    <li>{getResource('AverageWorkDays')}<span className="bold">: {hrm.averageWorkingDays}</span></li>
                    <li>{getResource('AverageSpentTime')}<span className="bold">: {hrm.averageSpentTime}</span></li>
                    <li>{getResource('MissingCheckInOut')}<span className="bold">: {hrm.missingCheckInCheckOut}</span></li>
                    <li>{getResource('LateInEarlyOut')}<span className="bold">: {hrm.totalLateInEarlyOut}</span></li>
                    <li>{getResource('UnpaidLeave')}<span className="bold">: {hrm.unpaidLeave}</span></li>
                    <li>{getResource('UnplannedLeave')}<span className="bold">: {hrm.unplannedAnnualLeave}</span></li>
                    <li>{getResource('SickLeave')}<span className="bold">: {hrm.sickLeave}</span></li>
                    <li>{getResource('Discipline')}<span className="bold">: {this.renderDiscipline(hrm.incident)}</span></li>
                </ul>
                {
                    this.state.isShowModal &&
                    <Discipline
                        memberId={memberId}
                        isShowModal={this.state.isShowModal}
                        onCloseModal={this.onCloseModal}
                    />
                }
            </div >
        );
    }
}