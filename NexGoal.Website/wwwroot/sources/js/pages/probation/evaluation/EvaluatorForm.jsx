import React, { Component, Fragment } from 'react';
import Select from 'react-select';
import axios from 'axios';
import Rating from 'react-rating';
import { flatMap, round, sumBy } from 'lodash';
import { Formik, getIn } from 'formik';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { OverlayTrigger } from 'react-bootstrap';

import { axiosError, resolveClientUrl, validateFormikField } from '../../../common-functions';
import { InputErrorCssClass, TextCenterCssClass, PostRequestHeaders } from '../../../constants';
import getResource from '../../../resourceHelper';
import ProbationResultOptions from './ProbationResultOptions';
import SingleValue from './SingleValue';
import EvaluationDetailTooltip from '../EvaluationDetailTooltip';
import { emptySymbol, fullSymbol } from '../RatingSymbol';
import NexPanel from '../../../shared/components/NexPanel';

export default class EvaluatorForm extends Component {
    evaluationDetailTitle = <Fragment>
        Quality of Work
        <OverlayTrigger placement="bottom" overlay={EvaluationDetailTooltip}>
            <i className="icon-info" />
        </OverlayTrigger>
    </Fragment>;

    renderPerformanceResult = qualified => <span className={qualified.value ? 'passed' : 'failed'}>{qualified.label}</span>;

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        setFieldError
    }) => {
        let score = 0;

        return (
            <Fragment>
                <NexPanel title={this.evaluationDetailTitle} enable={false}>
                    <table className="table table-bordered table-striped  quality-table">
                        <tbody>
                            {values.evaluations.map((kpiGroup, gIndex) => (
                                <Fragment key={kpiGroup.kpiGroupId}>
                                    <tr className="group-row">
                                        <td>{gIndex + 1}. {kpiGroup.name}</td>
                                        <td className={TextCenterCssClass}>{getResource('Employee')}</td>
                                        <td className={TextCenterCssClass}>{getResource('Evaluator')}</td>
                                    </tr>
                                    {kpiGroup.details.map((kpi, index) => {
                                        score += kpi.evaluatorPoint;

                                        return (
                                            <tr key={kpi.kpiId}>
                                                <td>{gIndex + 1}-{index + 1}. {kpi.kpiName}</td>
                                                <td className={TextCenterCssClass}><strong>{kpi.selfPoint}</strong></td>
                                                <td className={TextCenterCssClass}>
                                                    {this.props.probationer.canEdit &&
                                                        <Rating initialRating={kpi.evaluatorPoint / 1.11 || 0}
                                                            emptySymbol={emptySymbol}
                                                            fullSymbol={fullSymbol}
                                                            onChange={value => {
                                                                setFieldValue(`evaluations.${gIndex}.details.${index}.evaluatorPoint`, round(value * 1.11, 2));
                                                                setFieldError(`evaluations.${gIndex}.details.${index}.evaluatorPoint`, null);
                                                            }}
                                                            className={`rating ${getIn(touched, `evaluations.${gIndex}.details.${index}.evaluatorPoint`)
                                                                && getIn(errors, `evaluations.${gIndex}.details.${index}.evaluatorPoint`)
                                                                && InputErrorCssClass}`}
                                                        />
                                                    }
                                                    <strong>{kpi.evaluatorPoint}</strong>
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </Fragment>
                            ))}

                            <tr className="score">
                                <td className="text-center text-bold">Score</td>
                                <td className={TextCenterCssClass}>
                                    {this.props.probationer.selfScore &&
                                        <span className="score-badge"><span>{this.props.probationer.selfScore}</span>/100 pts</span>
                                    }
                                </td>
                                <td className={TextCenterCssClass}>
                                    {score > 0 &&
                                        <span className="score-badge"><span>{round(score)}</span>/100 pts</span>
                                    }
                                </td>
                            </tr>
                            <tr className="score">
                                <td className="text-center text-bold">{getResource('Result')}</td>
                                <td className="empty-cell" />
                                <td className={TextCenterCssClass}>
                                    {this.props.probationer.canEdit ?
                                        <Select name="isQualifiedByEvaluator"
                                            className={`react-select-container ${!validateFormikField(touched, errors, 'isQualifiedByEvaluator') && InputErrorCssClass}`}
                                            classNamePrefix="react-select"
                                            value={values.isQualifiedByEvaluator}
                                            options={ProbationResultOptions} placeholder=""
                                            isClearable={false} isSearchable={false}
                                            components={{ SingleValue }}
                                            onChange={value => {
                                                setFieldValue('isQualifiedByEvaluator', value);
                                                setFieldError(`isQualifiedByEvaluator`, null);
                                            }}
                                        />
                                        :
                                        values.isQualifiedByEvaluator && this.renderPerformanceResult(values.isQualifiedByEvaluator)
                                    }
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
                                    <textarea className={!validateFormikField(touched, errors, 'comment') && InputErrorCssClass} rows="7"
                                        value={values.comment}
                                        onChange={event => {
                                            handleChange(event);
                                            setFieldError(`comment`, null);
                                        }}
                                        name="comment" readOnly={!this.props.probationer.canEdit}
                                    />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </NexPanel>

                <div className="button-group">
                    <a className="btn btn-default btn-cancel" href="/member/index?activeTab=4">{getResource('Back')}</a>
                    {this.props.probationer.canEdit &&
                        <button className="btn btn-primary"  type="button" onClick={handleSubmit}>{getResource('Submit')}</button>
                    }
                </div>
            </Fragment>

        );
    }; 

    validateData = values => {
        const errors = {};
        let isValid = true;

        if (!values.comment) {
            errors.comment = 'Required';
            isValid = false;
        }

        if (!values.isQualifiedByEvaluator) {
            errors.isQualifiedByEvaluator = 'Required';
            isValid = false;
        }

        values.evaluations.forEach((eva, indexEva) => {
            if (!errors.evaluations) {
                errors.evaluations = [];
            }
            errors.evaluations[indexEva] = {};

            eva.details.forEach((kpi, indexKPI) => {
                if (!errors.evaluations[indexEva].details) {
                    errors.evaluations[indexEva].details = [];
                }

                errors.evaluations[indexEva].details[indexKPI] = {};
                if (!kpi.evaluatorPoint) {
                    isValid = false;
                    errors.evaluations[indexEva].details[indexKPI].evaluatorPoint = 'Required';
                }
            })
        })

        if (!isValid) {
            toast.error('Please input all required fields.');
        }

        return errors;
    };

    handleSubmit = (values, { setSubmitting }) => {
        const evaluations = flatMap(values.evaluations, kpiGroup => kpiGroup.details.map(kpi => ({
            employeeId: kpi.employeeId,
            kpiId: kpi.kpiId,
            point: kpi.evaluatorPoint
        })));

        const submitData = {
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
                ...submitData
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
    };

    render() {
        return (
            <Formik
                initialValues={{
                    comment: this.props.probationer.evaluatorComment,
                    evaluations: this.props.evaluationDetails,
                    isQualifiedByEvaluator: this.props.probationer.isQualifiedByEvaluator !== null ?
                        ProbationResultOptions.find(x => x.value === this.props.probationer.isQualifiedByEvaluator) : null
                }}
                validateOnChange={false}
                render={this.renderForm}
                validate={this.validateData}
                onSubmit={this.handleSubmit}
            />
        );
    }
}