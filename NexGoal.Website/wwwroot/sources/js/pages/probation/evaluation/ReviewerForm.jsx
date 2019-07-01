import React, { Component, Fragment } from 'react';

import Select from 'react-select';
import axios from 'axios';
import * as Yup from 'yup';
import { Formik } from 'formik';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { OverlayTrigger } from 'react-bootstrap';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ProbationResultOptions from './ProbationResultOptions';
import SingleValue from './SingleValue';
import EvaluationDetailTooltip from '../EvaluationDetailTooltip';
import NexPanel from '../../../shared/components/NexPanel';
import { DateFormat, PerformanceProcess } from '../../../constants';
import Announcement from './Announcement';
import { string } from 'prop-types';

export default class ReviewerForm extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showAnnouncement: !this.props.probationer.canEdit,
            announcementMessage: this.getAnnouncementMessage()
        };
    }

    getAnnouncementMessage = () => {
        const reviewerProcess = this.props.probationer.settings.find(x => x.processId === PerformanceProcess.Reviewer);

        return `Please be informed that these information might not be the final one, and you will not be able to make any changes until ${reviewerProcess.fromDate.format(DateFormat)}. Thank you for your concern`;
    }

    updatePoint = (kpi, value) => {
        const evaluationDetails = this.state.evaluationDetails.slice();
        kpi.evaluatorPoint = value * 1.11;

        this.setState({ evaluationDetails });
        this.calcuateAverageGrade();
    }

    renderKPI = (kpi, index) => (
        <tr key={kpi.kpiId}>
            <td>{index + 1}. {kpi.kpiName}</td>
            <td className="text-center"><strong>{kpi.selfPoint}</strong></td>
            <td className="text-center"><strong>{kpi.evaluatorPoint}</strong></td>
        </tr>
    )

    renderKPIGroup = (kpiGroup, index) => (
        <Fragment key={kpiGroup.kpiGroupId}>
            <tr className="group-row">
                <td>{index + 1}. {kpiGroup.name}</td>
                <td className="text-center">{getResource('Employee')}</td>
                <td className="text-center">{getResource('Evaluator')}</td>
            </tr>
            {kpiGroup.details.map(this.renderKPI)}
        </Fragment>
    )

    schema = Yup.object().shape({
        isQualified: Yup.object().required()
    })

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        return (
            <Fragment>
                <NexPanel title="Performance Result">
                    <table className="table table-bordered table-striped result-table">
                        <tbody>
                            <tr className="group-row">
                                <td colSpan="4">A. RESULT</td>
                            </tr>
                            <tr>
                                <td className="text-center text-title">{getResource('Self')}</td>
                                <td className="text-center text-title">{getResource('Score')}</td>
                                <td className="text-center text-score">{this.props.probationer.selfScore && `${this.props.probationer.selfScore} pts`}</td>
                                <td className="comment-cell comment-text">{this.props.probationer.selfComment}</td>
                            </tr>
                            <tr>
                                <td className="text-center text-title" rowSpan="2">{getResource('Evaluator')}</td>
                                <td className="text-center text-title">{getResource('Score')}</td>
                                <td className="text-center text-score">{this.props.probationer.evaluatorScore && `${this.props.probationer.evaluatorScore} pts`}</td>
                                <td className="comment-cell comment-text" rowSpan="2">{this.props.probationer.evaluatorComment}</td>
                            </tr>
                            <tr>
                                <td className="text-center text-title">{getResource('Result')}</td>
                                <td className="text-center text-score">
                                    <span>
                                        {this.props.probationer.isQualifiedByEvaluator !== null ? this.props.probationer.isQualifiedByEvaluator ? 'Passed' : 'Failed' : '-'}
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td className="text-center text-title">{getResource('Reviewer')}</td>
                                <td className="text-center text-title">{getResource('FinalResult')}</td>
                                <td className="text-score">
                                    {this.props.probationer.canEdit &&
                                        <Select name="isQualified"
                                            className={`react-select-container ${touched.isQualified && errors.isQualified && 'input-validation-error'}`}
                                            classNamePrefix="react-select" isDisabled={!this.props.probationer.canEdit}
                                            value={values.isQualified}
                                            options={ProbationResultOptions} placeholder=""
                                            isClearable={false} isSearchable={false}
                                            components={{ SingleValue }}
                                            onChange={value => setFieldValue('isQualified', value)}
                                        />
                                    }
                                </td>
                                <td className="comment-cell input-comment">
                                    {this.props.probationer.canEdit &&
                                        <textarea className={`${touched.comment && errors.comment && 'input-validation-error'}`}
                                            value={values.comment} onChange={handleChange} name="comment" />
                                    }
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table className="table table-bordered table-striped">
                        <tbody>

                            <tr className="group-row">
                                <td colSpan="3">
                                    B. Details
                                <OverlayTrigger placement="bottom" overlay={EvaluationDetailTooltip}>
                                        <i className="icon-info" />
                                    </OverlayTrigger>
                                </td>
                            </tr>
                            {this.props.evaluationDetails.map(this.renderKPIGroup)}
                            <tr className="score">
                                <td className="text-center"><strong>Score</strong></td>
                                <td className="text-center">
                                    {this.props.probationer.selfScore &&
                                        <span className="score-badge"><span>{this.props.probationer.selfScore}</span>/100 pts</span>
                                    }
                                </td>
                                <td className="text-center">
                                    {this.props.probationer.evaluatorScore &&
                                        <span className="score-badge"><span>{this.props.probationer.evaluatorScore}</span>/100 pts</span>
                                    }
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </NexPanel>
                <div className="button-group">
                    <a className="btn btn-default btn-cancel" href="/member/index?activeTab=4">{getResource('Back')}</a>
                    {this.props.probationer.canEdit &&
                        <button className="btn btn-primary" disabled={isSubmitting} type="button" onClick={handleSubmit}>{getResource('Submit')}</button>
                    }
                </div>
            </Fragment >
        );
    }

    handleSubmit = (values, { setErrors, setSubmitting }) => {
        axios({
            url: resolveClientUrl('Probation/EditReviewerEvaluation'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                employeeId: this.props.probationer.employeeId,
                comment: values.comment,
                isQualified: values.isQualified.value
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                toast.error(data.message);
            }

            setSubmitting(false);
        }).catch(axiosError);
    }

    handleCloseAnnouncement = () => this.setState({ showAnnouncement: false });

    render() {
        return (
            <Fragment>
                <Formik
                    initialValues={{
                        comment: this.props.probationer.reviewerComment,
                        isQualified: this.props.probationer.isQualified ? ProbationResultOptions.find(x => x.value === this.props.probationer.isQualified) : null
                    }}
                    render={this.renderForm}
                    validationSchema={this.schema}
                    onSubmit={this.handleSubmit}
                />
                <Announcement content={this.state.announcementMessage} showModal={this.state.showAnnouncement} onCloseModal={this.handleCloseAnnouncement} />
            </Fragment>
        );
    }
}