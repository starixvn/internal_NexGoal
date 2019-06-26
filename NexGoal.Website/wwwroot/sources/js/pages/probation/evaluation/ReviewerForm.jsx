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

export default class ReviewerForm extends Component {
    constructor(props) {
        super(props);
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
            <td className="text-center">{kpi.selfPoint}</td>
            <td className="text-center">{kpi.evaluatorPoint}</td>
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
                    <table className="table table-bordered table-striped">
                        <tbody>
                            <tr className="group-row">
                                <td colSpan="4">A. Result</td>
                            </tr>
                            <tr>
                                <td className="text-center">{getResource('Self')}</td>
                                <td className="text-center">{getResource('Score')}</td>
                                <td className="text-center">{this.props.probationer.selfScore && `${this.props.probationer.selfScore} pts`}</td>
                                <td className="comment-cell"><p className="comment">{this.props.probationer.selfComment}</p></td>
                            </tr>
                            <tr>
                                <td className="text-center" rowSpan="2">{getResource('Evaluator')}</td>
                                <td className="text-center">{getResource('Score')}</td>
                                <td className="text-center">{this.props.probationer.evaluatorScore && `${this.props.probationer.evaluatorScore} pts`}</td>
                                <td className="comment-cell" rowSpan="2"><p className="comment">{this.props.probationer.evaluatorComment}</p></td>
                            </tr>
                            <tr>
                                <td className="text-center">{getResource('Result')}</td>
                                <td className="text-center">
                                    <span>
                                        {this.props.probationer.isQualifiedByEvaluator !== null ? this.props.probationer.isQualifiedByEvaluator ? 'Passed' : 'Failed' : '-'}
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td className="text-center">{getResource('Reviewer')}</td>
                                <td className="text-center">{getResource('FinalResult')}</td>
                                <td>
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
                                <td className="comment-cell">
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
                                <td className="text-center">Score</td>
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
                <div className="btn-group">
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

    render() {
        return (
            <Formik
                initialValues={{
                    comment: this.props.probationer.reviewerComment,
                    isQualified: this.props.probationer.isQualified ? ProbationResultOptions.find(x => x.value === this.props.probationer.isQualified) : null
                }}
                render={this.renderForm}
                validationSchema={this.schema}
                onSubmit={this.handleSubmit}
            />
        );
    }
}