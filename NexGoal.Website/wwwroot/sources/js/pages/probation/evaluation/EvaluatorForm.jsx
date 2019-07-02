import React, { Component, Fragment } from 'react';

import Select from 'react-select';
import axios from 'axios';
import * as Yup from 'yup';
import Rating from 'react-rating';
import { flatMap, round, sumBy } from 'lodash';
import { Formik, getIn } from 'formik';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { OverlayTrigger } from 'react-bootstrap';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ProbationResultOptions from './ProbationResultOptions';
import SingleValue from './SingleValue';
import EvaluationDetailTooltip from '../EvaluationDetailTooltip';
import { emptySymbol, fullSymbol } from '../RatingSymbol';
import NexPanel from '../../../shared/components/NexPanel';

export default class EvaluatorForm extends Component {
    constructor(props) {
        super(props);
    }

    schema = Yup.object().shape({
        comment: Yup.string().required(),
        evaluations: Yup.array().of(
            Yup.object().shape({
                details: Yup.array().of(
                    Yup.object().shape({
                        evaluatorPoint: Yup.number().required()
                    })
                )
            })
        ),
        isQualifiedByEvaluator: Yup.object().required()
    })

    evaluationDetailTitle = <Fragment>
        B. QUALITY OF WORK
        <OverlayTrigger placement="bottom" overlay={EvaluationDetailTooltip}>
            <i className="icon-info" />
        </OverlayTrigger>
    </Fragment>

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        let score = 0;

        return (
            <Fragment>
                <NexPanel title={this.evaluationDetailTitle}>
                    <table className="table table-bordered table-striped  quality-table">
                        <tbody>
                            {values.evaluations.map((kpiGroup, gIndex) => (
                                <Fragment key={kpiGroup.kpiGroupId}>
                                    <tr className="group-row">
                                        <td>{gIndex + 1}. {kpiGroup.name}</td>
                                        <td className="text-center">{getResource('Employee')}</td>
                                        <td className="text-center">{getResource('Evaluator')}</td>
                                    </tr>
                                    {kpiGroup.details.map((kpi, index) => {
                                        score += kpi.evaluatorPoint;

                                        return (
                                            <tr key={kpi.kpiId}>
                                                <td>{index + 1}. {kpi.kpiName}</td>
                                                <td className="text-center"><strong>{kpi.selfPoint}</strong></td>
                                                <td className="text-center">
                                                    {this.props.probationer.canEdit &&
                                                        <Rating initialRating={kpi.evaluatorPoint / 1.11 || 0}
                                                            emptySymbol={emptySymbol}
                                                            fullSymbol={fullSymbol}
                                                            onChange={value => setFieldValue(`evaluations.${gIndex}.details.${index}.evaluatorPoint`, round(value * 1.11, 2))}
                                                            className={`rating ${getIn(touched, `evaluations.${gIndex}.details.${index}.evaluatorPoint`)
                                                                && getIn(errors, `evaluations.${gIndex}.details.${index}.evaluatorPoint`)
                                                                && 'input-validation-error'}`}
                                                        />
                                                    }
                                                    <strong>{kpi.evaluatorPoint}</strong>
                                                </td>
                                            </tr>
                                        )
                                    })}
                                </Fragment>
                            ))}

                            <tr className="score">
                                <td className="text-center text-bold">Score</td>
                                <td className="text-center">
                                    {this.props.probationer.selfScore &&
                                        <span className="score-badge"><span>{this.props.probationer.selfScore}</span>/100 pts</span>
                                    }
                                </td>
                                <td className="text-center">
                                    {score > 0 &&
                                        <span className="score-badge"><span>{round(score)}</span>/100 pts</span>
                                    }
                                </td>
                            </tr>
                            <tr className="score">
                                <td className="text-center text-bold">{getResource('Result')}</td>
                                <td className="empty-cell" />
                                <td>
                                    <Select name="isQualifiedByEvaluator"
                                        className={`react-select-container ${touched.isQualifiedByEvaluator && errors.isQualifiedByEvaluator && 'input-validation-error'}`}
                                        classNamePrefix="react-select" isDisabled={!this.props.probationer.canEdit}
                                        value={values.isQualifiedByEvaluator}
                                        options={ProbationResultOptions} placeholder=""
                                        isClearable={false} isSearchable={false}
                                        components={{ SingleValue }}
                                        onChange={value => setFieldValue('isQualifiedByEvaluator', value)}
                                    />
                                </td>
                            </tr>
                            <tr>
                                <td className="comment-title" colSpan="3">Employee's Comment</td>
                            </tr>
                            <tr>
                                <td colSpan="3" className="comment-cell">
                                    <p>{this.props.probationer.selfComment}</p>
                                </td>
                            </tr>
                            <tr>
                                <td className="comment-title" colSpan="3">Evaluator's Comment</td>
                            </tr>
                            <tr>
                                <td colSpan="3" className="comment-cell">
                                    <textarea className={`${touched.comment && errors.comment && 'input-validation-error'}`} rows="7"
                                        value={values.comment} onChange={handleChange} name="comment" readOnly={!this.props.probationer.canEdit}
                                    />
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
            </Fragment>

        );
    }

    handleSubmit = (values, { setErrors, setSubmitting }) => {
        const evaluations = flatMap(values.evaluations, (kpiGroup) => kpiGroup.details.map(kpi => ({
            employeeId: kpi.employeeId,
            kpiId: kpi.kpiId,
            point: kpi.evaluatorPoint
        })));

        const data = {
            employeeId: this.props.probationer.employeeId,
            comment: values.comment,
            evaluations: evaluations,
            totalScore: round(sumBy(evaluations, 'point')),
            isQualifiedByEvaluator: values.isQualifiedByEvaluator.value
        };

        axios({
            url: resolveClientUrl('Probation/EditEvaluatorEvaluation'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                ...data
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
                    comment: this.props.probationer.evaluatorComment,
                    evaluations: this.props.evaluationDetails,
                    isQualifiedByEvaluator: this.props.probationer.isQualifiedByEvaluator ? ProbationResultOptions.find(x => x.value === this.props.probationer.isQualifiedByEvaluator) : null
                }}
                render={this.renderForm}
                validationSchema={this.schema}
                onSubmit={this.handleSubmit}
            />
        );
    }
}