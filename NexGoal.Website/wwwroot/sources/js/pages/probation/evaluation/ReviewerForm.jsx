import React, { Component, Fragment } from 'react';
import Select from 'react-select';
import axios from 'axios';
import { object, string } from 'yup';
import { Formik } from 'formik';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { OverlayTrigger } from 'react-bootstrap';

import { axiosError, resolveClientUrl, validateFormikField } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ProbationResultOptions from './ProbationResultOptions';
import SingleValue from './SingleValue';
import EvaluationDetailTooltip from '../EvaluationDetailTooltip';
import NexPanel from '../../../shared/components/NexPanel';
import { DateFormat, PerformanceProcess, PostRequestHeaders } from '../../../constants';
import Announcement from './Announcement';
import Paragraph from './Paragraph';

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
        const message = 'Please be informed that these information might not be the final one, and you will not be able to make any changes until';
        return `${message} ${reviewerProcess.fromDate.format(DateFormat)}. Thank you for your concern`;
    }

    updatePoint = (kpi, value) => {
        const evaluationDetails = this.state.evaluationDetails.slice();
        kpi.evaluatorPoint = value * 1.11;

        this.setState({ evaluationDetails });
        this.calcuateAverageGrade();
    }

    renderKPI = (kpi, index) => (
        <tr key={kpi.kpiId}>
            <td>{kpi.kpiGroupId}-{index + 1}. {kpi.kpiName}</td>
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

    schema = object().shape({
        isQualified: object().required(),
        comment: string().required()
    })

    renderPerformanceResult = probationer => {
        return (
            <span className={probationer.isQualifiedByEvaluator ? 'passed' : 'failed'}>
                {probationer.isQualifiedByEvaluator ? 'Passed' : 'Failed'}
            </span>
        );
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        const probationer = this.props.probationer;
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
                                <td className="text-center text-score">{probationer.selfScore && `${probationer.selfScore} pts`}</td>
                                <td className="comment-cell comment-text">
                                    <Paragraph content={probationer.selfComment} />
                                </td>
                            </tr>
                            <tr>
                                <td className="text-center text-title" rowSpan="2">
                                    {getResource('Evaluator')}
                                    <div>({probationer.settings.find(x => x.processId === PerformanceProcess.Evaluator).evaluatorNickName})</div>
                                </td>
                                <td className="text-center text-title">{getResource('Score')}</td>
                                <td className="text-center text-score">{probationer.evaluatorScore && `${probationer.evaluatorScore} pts`}</td>
                                <td className="comment-cell comment-text" rowSpan="2">
                                    <Paragraph content={probationer.evaluatorComment} />
                                </td>
                            </tr>
                            <tr>
                                <td className="text-center text-title">{getResource('Result')}</td>
                                <td className="text-center text-score">{this.renderPerformanceResult(probationer)}</td>
                            </tr>
                            <tr>
                                <td className="text-center text-title">
                                    {getResource('Reviewer')}
                                    <div>({probationer.settings.find(x => x.processId === PerformanceProcess.Reviewer).evaluatorNickName})</div>
                                </td>
                                <td className="text-center text-title">{getResource('FinalResult')}</td>
                                <td className="text-score">
                                    {
                                        probationer.canEdit &&
                                        <Select name="isQualified"
                                            className={`react-select-container ${!validateFormikField(touched, errors, 'isQualified') && 'input-validation-error'}`}
                                            classNamePrefix="react-select" isDisabled={!probationer.canEdit}
                                            value={values.isQualified}
                                            options={ProbationResultOptions} placeholder=""
                                            isClearable={false} isSearchable={false}
                                            components={{ SingleValue }}
                                            onChange={value => setFieldValue('isQualified', value)}
                                        />
                                    }
                                </td>
                                <td className="comment-cell input-comment">
                                    {
                                        probationer.canEdit &&
                                        <textarea className={!validateFormikField(touched, errors, 'comment') && 'input-validation-error'}
                                            value={values.comment} onChange={handleChange} name="comment" rows="5"
                                        />
                                    }
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table className="table table-bordered table-striped">
                        <tbody>
                            <tr className="group-row">
                                <td colSpan="3">
                                    B. DETAILS
                                <OverlayTrigger placement="bottom" overlay={EvaluationDetailTooltip}>
                                        <i className="icon-info" />
                                    </OverlayTrigger>
                                </td>
                            </tr>
                            {this.props.evaluationDetails.map(this.renderKPIGroup)}
                            <tr className="score">
                                <td className="text-center"><strong>{getResource('Score')}</strong></td>
                                <td className="text-center">
                                    {
                                        probationer.selfScore &&
                                        <span className="score-badge"><span>{probationer.selfScore}</span>/100 pts</span>
                                    }
                                </td>
                                <td className="text-center">
                                    {
                                        probationer.evaluatorScore &&
                                        <span className="score-badge"><span>{probationer.evaluatorScore}</span>/100 pts</span>
                                    }
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </NexPanel>
                <div className="button-group">
                    <a className="btn btn-default btn-cancel" href="/member/index?activeTab=4">{getResource('Back')}</a>
                    {
                        probationer.canEdit &&
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
            headers: PostRequestHeaders
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
                        isQualified: this.props.probationer.isQualified !== null ? ProbationResultOptions.find(x => x.value === this.props.probationer.isQualified) : null
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